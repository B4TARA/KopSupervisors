using System.Data;
using System.Xml;
using ExcelDataReader;
using KOP.Common.Enums;
using KOP.DAL;
using KOP.DAL.Entities;
using KOP.DAL.Entities.AssessmentEntities;
using KOP.DAL.Entities.GradeEntities;
using KOP.DAL.Repositories;
using KOP.EmailService;
using KOP.Import.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using NLog;
using SystemRoles = KOP.Common.Enums.SystemRoles;

namespace KOP.Import
{
    public class ExportAndImport
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly IEmailSender _emailSender;
        private readonly IConfigurationRoot _config;

        private readonly string _additionalUsersServiceNumbersPath;
        private readonly string _usersInfosPath;
        private readonly string _usersStructuresPath;
        private readonly string _colsArrayPath;
        private readonly string _usersPassContainerPath;
        private readonly string _usersImgDownloadPath;
        private readonly string _emailIconPath;
        private readonly string _trainingEventsPath;

        private List<int> additionalUsersServiceNumbers = new();

        public ExportAndImport(DbContextOptions<ApplicationDbContext> options, IConfigurationRoot config)
        {
            _options = options;
            _config = config;
            //_emailSender = new EmailSender();

            _additionalUsersServiceNumbersPath = _config["FilePaths:AdditionalUsersServiceNumbersPath"] ?? "";
            _usersInfosPath = _config["FilePaths:UsersInfosPath"] ?? "";
            _usersStructuresPath = _config["FilePaths:UsersStructuresPath"] ?? "";
            _colsArrayPath = _config["FilePaths:ColsArrayPath"] ?? "";
            _usersPassContainerPath = _config["FilePaths:UsersPassContainerPath"] ?? "";
            _usersImgDownloadPath = _config["FilePaths:UsersImgDownloadPath"] ?? "";
            _emailIconPath = _config["FilePaths:EmailIconPath"] ?? "";
            _trainingEventsPath = _config["FilePaths:TrainingEventsPath"] ?? "";
        }

        public async Task TransferDataFromExcelToDatabase()
        {
            try
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                //var usersFromExcel = ProcessUsers();
                //await PutUsersInDatabase(usersFromExcel);
                //await BlockNonActiveUsers(usersFromExcel);
                //await CheckUsersForGradeProcess();
                await CheckForNotifications();
                //await ProcessTrainingEvents(_trainingEventsPath);
            }
            catch (Exception ex)
            {
                var addressee = new string[] { "ebaturel@mtb.minsk.by" };
                var messageBody = ex.Message;
                var errorMessage = new Message(addressee, "Ошибка импорта", messageBody, "Батурель Евгений Дмитриевич");
                await _emailSender.SendEmailAsync(errorMessage);
                _logger.Error(ex.Message);
            }
        }

        private List<User> ProcessUsers()
        {
            additionalUsersServiceNumbers = ReadAdditionalUsersServiceNumbersFromFile(_additionalUsersServiceNumbersPath);
            var usersFromExcel = ReadUsersStructuresFromFile(_usersStructuresPath);
            return ReadUsersInfosFromFile(_usersInfosPath, usersFromExcel);
        }

        private async Task PopulateUserWithModule(User userFromExcel, UnitOfWork uow, string parentSubdivisionName)
        {
            if (IsAdditionalSubdivision(userFromExcel.SubdivisionFromFile) && !IsMeetUserStructureBusinessRequirements(userFromExcel.StructureRole, userFromExcel.GradeGroup))
            {
                return;
            }

            var subdivision = await uow.Subdivisions.GetAsync(x => x.Name.Replace(" ", "").ToLower() == parentSubdivisionName.Replace(" ", "").ToLower());

            if (subdivision is null)
            {
                return;
            }

            userFromExcel.ParentSubdivision = subdivision;
        }

        private void PopulateUserWithXmlData(User userFromExcel, XmlDocument colsArrayDocument, XmlDocument usersPassContainerDocument)
        {
            if (userFromExcel.FullName is null)
            {
                return;
            }

            var parts = userFromExcel.FullName.Split(' ');
            var firstName = parts[0];
            var lastName = parts.Length > 1 ? parts[1] : "";
            var middleName = parts.Length > 2 ? parts[2] : "";

            var xpath = $"//value[contains(., '{firstName}') and contains(., '{lastName}') and contains(., '{middleName}')]";

            var userNode1 = colsArrayDocument.SelectSingleNode(xpath);

            if (userNode1 != null)
            {
                var obj = JObject.Parse(userNode1.InnerText);

                //var email = (string)obj["email"];
                var email = "ebaturel@mtb.minsk.by";
                var base64Image = (string)obj["pict_url"];
                var fileName = userFromExcel.FullName.Replace(" ", "");
                var imagePath = ImageUtilities.SaveBase64Image(base64Image, fileName, _usersImgDownloadPath);

                userFromExcel.ImagePath = imagePath;
                userFromExcel.Email = email;
            }

            var userNode2 = usersPassContainerDocument.SelectSingleNode(xpath);

            if (userNode1 != null && userNode2 != null)
            {
                var obj = JObject.Parse(userNode2.InnerText);

                var login = (string)obj["login"];
                var password = (string)obj["password"];

                userFromExcel.Login = login;
                userFromExcel.Password = password;
            }
        }

        private async Task PopulateUserWithRole(User userFromExcel, UnitOfWork uow)
        {
            // ПРОВЕРИТЬ ВЫДАЧУ РОЛИ СОТРУДНИКА ВСЕМ, КРОМЕ АДМ_АДМИНИСТРАЦИЯ
            userFromExcel.SystemRoles.Add(SystemRoles.Employee);

            if (userFromExcel.SubdivisionFromFile is null)
            {
                return;
            }
            else if (userFromExcel.SubdivisionFromFile.Contains("УМСТ "))
            {
                userFromExcel.SystemRoles.Add(SystemRoles.Umst);

                var rootSubdivisions = await uow.Subdivisions.GetAllAsync(x => x.NestingLevel == 1);

                userFromExcel.SubordinateSubdivisions = rootSubdivisions.ToList();
            }
            else if (userFromExcel.SubdivisionFromFile.Contains("ЦУП "))
            {
                userFromExcel.SystemRoles.Add(SystemRoles.Cup);

                var rootSubdivisions = await uow.Subdivisions.GetAllAsync(x => x.NestingLevel == 1);

                userFromExcel.SubordinateSubdivisions = rootSubdivisions.ToList();
            }
            else if (userFromExcel.SubdivisionFromFile.Contains("УРП "))
            {
                userFromExcel.SystemRoles.Add(SystemRoles.Urp);

                var rootSubdivisions = await uow.Subdivisions.GetAllAsync(x => x.NestingLevel == 1);

                userFromExcel.SubordinateSubdivisions = rootSubdivisions.ToList();
            }
            else if (userFromExcel.SubdivisionFromFile.Contains("УОП "))
            {
                userFromExcel.SystemRoles.Add(SystemRoles.Uop);

                var rootSubdivisions = await uow.Subdivisions.GetAllAsync(x => x.NestingLevel == 1);

                userFromExcel.SubordinateSubdivisions = rootSubdivisions.ToList();
            }
        }

        private async Task PutUsersInDatabase(List<User> usersFromExcel)
        {
            var invalidUsers = new List<User>();

            using (var uow = new UnitOfWork(new ApplicationDbContext(_options)))
            {
                var _userValidator = new UserValidator(uow);

                foreach (var userFromExcel in usersFromExcel)
                {
                    try
                    {
                        var result = _userValidator.Validate(userFromExcel);
                        if (!result.IsValid)
                        {
                            invalidUsers.Add(userFromExcel);

                            continue;
                        }

                        await PopulateUserWithRole(userFromExcel, uow);
                        await PopulateUserWithModule(userFromExcel, uow, userFromExcel.SubdivisionFromFile);

                        var existingUser = await uow.Users.GetAsync(x => x.ServiceNumber == userFromExcel.ServiceNumber, includeProperties: "ParentSubdivision");

                        if (existingUser == null)
                        {
                            await uow.Users.AddAsync(userFromExcel);
                            await uow.CommitAsync();
                            continue;
                        }

                        existingUser.FullName = userFromExcel.FullName;
                        existingUser.Position = userFromExcel.Position;
                        existingUser.GradeGroup = userFromExcel.GradeGroup;
                        existingUser.HireDate = userFromExcel.HireDate;
                        existingUser.ContractStartDate = userFromExcel.ContractStartDate;
                        existingUser.ContractEndDate = userFromExcel.ContractEndDate;
                        existingUser.Login = userFromExcel.Login;
                        existingUser.Password = userFromExcel.Password;
                        existingUser.Email = userFromExcel.Email;
                        existingUser.ImagePath = userFromExcel.ImagePath;
                        existingUser.ParentSubdivision = userFromExcel.ParentSubdivision;
                        //existingUser.SystemRoles = userFromExcel.SystemRoles;

                        uow.Users.Update(existingUser);
                        await uow.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        await uow.RollbackAsync();
                        _logger.Error(ex.Message);
                        continue;
                    }
                }
            }
        }

        private async Task CheckForNotifications()
        {
            using (var uow = new UnitOfWork(new ApplicationDbContext(_options)))
            {
                var today = TermManager.GetDate();
                var users = await uow.Users.GetAllAsync(includeProperties: "Grades");
                var mails = await uow.Mails.GetAllAsync();
                //var pendingAssessmentResults = await uow.AssessmentResults.GetAllAsync(x => x.SystemStatus == SystemStatuses.PENDING);

                foreach (var user in users.Where(x => 
                    //x.Id == 123 || // Полякова
                    x.Id == 176 || // Затковская
                    x.Id == 156 //|| // Цедрик
                    //x.Id == 157 || // Глушко
                    //x.Id == 198 || // Корочкина
                    //x.Id == 195 || // Соломыкина
                    //x.Id == 178 || // Шастиловская
                    //x.Id == 180 || // Гришкина
                    //x.Id == 182 || // Чернышова
                    //x.Id == 188 || // Сакирина
                    //x.Id == 142    // Сурдо
                    ))
                {
                    try
                    {
                        // Руководителям 1 и 10 числа месяца оценки
                        // Оценить КК, УК и Назначить группу оценки КК
                        if ((today.Day == 1 || today.Day == 10) && user.SystemRoles.Contains(SystemRoles.Supervisor))
                        {
                            var subordinateUsers = await GetSubordinateUsers(user.Id);
                            var subordinateUsersWithThisMonthPendingGrade = subordinateUsers
                                .Where(x => x.Grades
                                    .Any(x =>
                                        x.DateOfCreation.Month == today.Month &&
                                        x.DateOfCreation.Year == today.Year &&
                                        x.SystemStatus == SystemStatuses.PENDING));

                            // Есть подчиненные с назначенными оценками в текущем месяце
                            if (subordinateUsersWithThisMonthPendingGrade.Any())
                            {
                                Mail? mail = null;

                                if (today.Day == 1)
                                {
                                    mail = mails.FirstOrDefault(x => x.Code == MailCodes.CreateAssessmentGroupAndAssessEmployeeNotification);
                                }
                                else if (today.Day == 10)
                                {
                                    mail = mails.FirstOrDefault(x => x.Code == MailCodes.CreateAssessmentGroupAndAssessEmployeeReminder);
                                }

                                if (mail is null)
                                {
                                    var addressee = new string[] { "ebaturel@mtb.minsk.by" };
                                    var messageBody = "Не найдено сообщение для отправки непосредственным руковолителям 1 или 10 числа месяца оценки";
                                    var errorMessage = new Message(addressee, "Ошибка импорта", messageBody, "Батурель Евгений Дмитриевич");
                                    await _emailSender.SendEmailAsync(errorMessage);
                                    _logger.Error(messageBody);
                                }

                                var message = new Message(new string[] { user.Email }, mail.Title, mail.Body, user.FullName);
                                await _emailSender.SendEmailAsync(message);
                            }
                        }
                        // Оцениваемым сотрудникам 1 и 15 числа месяца оценки
                        // Провести самооценку КК, УК и заполнить результаты деятельности
                        if ((today.Day == 1 || today.Day == 15) && user.SystemRoles.Contains(SystemRoles.Employee))
                        {
                            var thisMonthPendingGrade = user.Grades
                                .FirstOrDefault(x =>
                                    x.DateOfCreation.Month == today.Month &&
                                    x.DateOfCreation.Year == today.Year &&
                                    x.SystemStatus == SystemStatuses.PENDING);

                            // Есть назначенные оценки в текущем месяце
                            if (thisMonthPendingGrade != null)
                            {
                                Mail? mail = null;

                                if (today.Day == 1)
                                {
                                    mail = mails.FirstOrDefault(x => x.Code == MailCodes.CreateStrategicTasksAndSelfAssessmentNotification);
                                }
                                else if (today.Day == 15)
                                {
                                    mail = mails.FirstOrDefault(x => x.Code == MailCodes.CreateStrategicTasksAndSelfAssessmentReminder);
                                }

                                if (mail is null)
                                {
                                    var addressee = new string[] { "ebaturel@mtb.minsk.by" };
                                    var messageBody = "Не найдено сообщение для отправки оцениваемым сотрудникам 1 или 15 числа месяца оценки";
                                    var errorMessage = new Message(addressee, "Ошибка импорта", messageBody, "Батурель Евгений Дмитриевич");
                                    await _emailSender.SendEmailAsync(errorMessage);
                                    _logger.Error(messageBody);
                                }

                                var message = new Message(new string[] { user.Email }, mail.Title, mail.Body, user.FullName);
                                await _emailSender.SendEmailAsync(message);
                            }
                        }
                        // Ответственным за заполнение показателей (УМСТ, ЦУП, УРП) 1 и 15 числа месяца оценки
                        // Заполнить критерии
                        if (today.Day == 1 || today.Day == 15)
                        {
                            var isInRole = user.SystemRoles.Contains(SystemRoles.Umst) ||
                                user.SystemRoles.Contains(SystemRoles.Cup) ||
                                user.SystemRoles.Contains(SystemRoles.Urp);

                            if (isInRole)
                            {
                                var usersWithThisMonthPendingGrade = users
                                   .Where(x => x.Grades
                                       .Any(x =>
                                           x.DateOfCreation.Month == today.Month &&
                                           x.DateOfCreation.Year == today.Year &&
                                           x.SystemStatus == SystemStatuses.PENDING));

                                // Есть сотрудники с назначенными оценками в текущем месяце
                                if (usersWithThisMonthPendingGrade.Any())
                                {
                                    Mail? mail = null;

                                    if (today.Day == 1)
                                    {
                                        mail = mails.FirstOrDefault(x => x.Code == MailCodes.CreateAssessmentCriteriaNotification);
                                    }
                                    else if (today.Day == 10)
                                    {
                                        mail = mails.FirstOrDefault(x => x.Code == MailCodes.CreateAssessmentCriteriaReminder);
                                    }

                                    if (mail is null)
                                    {
                                        var addressee = new string[] { "ebaturel@mtb.minsk.by" };
                                        var messageBody = "Не найдено сообщение для отправки ответственным за заполнение критериев 1 или 15 числа месяца оценки";
                                        var errorMessage = new Message(addressee, "Ошибка импорта", messageBody, "Батурель Евгений Дмитриевич");
                                        await _emailSender.SendEmailAsync(errorMessage);
                                        _logger.Error(messageBody);
                                    }

                                    var message = new Message(new string[] { user.Email }, mail.Title, mail.Body, user.FullName);
                                    await _emailSender.SendEmailAsync(message);
                                }
                            }
                        }
                        // Группе оценки КК + HR (Отдел развития УРП) 15 числа месяца оценки
                        // Оценить КК
                        if (today.Day == 15)
                        {
                            var appointedThisMonthCorporateAssessmentResults = await uow.AssessmentResults
                                .GetAllAsync(x =>
                                    x.JudgeId == user.Id && // Пользователь является оценщиком
                                    x.Assessment.UserId != user.Id && // Не самооценка
                                    x.DateOfCreation.Month == today.Month &&
                                    x.DateOfCreation.Year == today.Year &&
                                    x.SystemStatus == SystemStatuses.PENDING &&
                                    x.Assessment.AssessmentType.SystemAssessmentType == SystemAssessmentTypes.СorporateСompetencies, includeProperties: "Assessment");

                            foreach (var assessmentResult in appointedThisMonthCorporateAssessmentResults)
                            {
                                var userId = assessmentResult.Assessment.UserId;
                                var supervisorForUser = GetSupervisorForUser(userId);

                                // Если пользователь является непосредственным руководителем оцениваемого
                                if (supervisorForUser != null && supervisorForUser.Id == user.Id)
                                {
                                    continue;
                                }

                                var mail = mails.FirstOrDefault(x => x.Code == MailCodes.CreateCorporateCompeteciesAssessmentNotification);
                                if (mail is null)
                                {
                                    var addressee = new string[] { "ebaturel@mtb.minsk.by" };
                                    var messageBody = "Не найдено сообщение для отправки оценщикам 15 числа месяца оценки";
                                    var errorMessage = new Message(addressee, "Ошибка импорта", messageBody, "Батурель Евгений Дмитриевич");
                                    await _emailSender.SendEmailAsync(errorMessage);
                                    _logger.Error(messageBody);
                                }

                                var message = new Message(new string[] { user.Email }, mail.Title, mail.Body, user.FullName);
                                //await _emailSender.SendEmailAsync(message, _emailIconPath);
                            }
                        }
                        // УРП 1 числа месяца, следующего за месяцем оценки
                        // Оставить выводы по криетриям: результаты деятельности, КПЭ, квалификация
                        if (today.Day == 1 && user.SystemRoles.Contains(SystemRoles.Urp))
                        {
                            var usersWithPreviousMonthPendingGrade = users
                                .Where(x => x.Grades
                                    .Any(x =>
                                        x.DateOfCreation.AddMonths(1).Month == today.Month &&
                                        x.DateOfCreation.Year == today.Year &&
                                        x.SystemStatus == SystemStatuses.PENDING));

                            // Есть сотрудники с назначенными оценками в прошлом месяце
                            if (usersWithPreviousMonthPendingGrade.Any())
                            {
                                var mail = mails.FirstOrDefault(x => x.Code == MailCodes.CreateConclusionsAndCheckGradeNotification);
                                if (mail is null)
                                {
                                    var addressee = new string[] { "ebaturel@mtb.minsk.by" };
                                    var messageBody = "Не найдено сообщение для отправки УРП 1 числа месяца, следующего за месяцем оценки";
                                    var errorMessage = new Message(addressee, "Ошибка импорта", messageBody, "Батурель Евгений Дмитриевич");
                                    await _emailSender.SendEmailAsync(errorMessage);
                                    _logger.Error(messageBody);
                                }

                                var message = new Message(new string[] { user.Email }, mail.Title, mail.Body, user.FullName);
                                //await _emailSender.SendEmailAsync(message, _emailIconPath);
                            }
                        }
                        // Непосредственным руководителям 5 и 8 числа месяца, следующего за месяцем оценки
                        // Оставить ОС от руководителя и ознакомиться с результатами оценки
                        if ((today.Day == 5 || today.Day == 8) && user.SystemRoles.Contains(SystemRoles.Supervisor))
                        {
                            var subordinateUsers = await GetSubordinateUsers(user.Id);
                            var subordinateUsersWithPreviousMonthPendingGrade = subordinateUsers
                                .Where(x => x.Grades
                                    .Any(x =>
                                        x.DateOfCreation.AddMonths(1).Month == today.Month &&
                                        x.DateOfCreation.Year == today.Year &&
                                        x.SystemStatus == SystemStatuses.PENDING));

                            // Есть сотрудники с назначенными оценками в прошлом месяце
                            if (subordinateUsersWithPreviousMonthPendingGrade.Any())
                            {
                                Mail? mail = null;

                                if (today.Day == 5)
                                {
                                    mail = mails.FirstOrDefault(x => x.Code == MailCodes.CreateValueJudgmentAndApproveEmployeeNotification);
                                }
                                else if (today.Day == 8)
                                {
                                    mail = mails.FirstOrDefault(x => x.Code == MailCodes.CreateValueJudgmentAndApproveEmployeeReminder);
                                }

                                if (mail is null)
                                {
                                    var addressee = new string[] { "ebaturel@mtb.minsk.by" };
                                    var messageBody = "Не найдено сообщение для отправки непосредственным руковолителям 5 или 8 числа месяца, следующего за месяцем оценки";
                                    var errorMessage = new Message(addressee, "Ошибка импорта", messageBody, "Батурель Евгений Дмитриевич");
                                    await _emailSender.SendEmailAsync(errorMessage);
                                    _logger.Error(messageBody);
                                }

                                var message = new Message(new string[] { user.Email }, mail.Title, mail.Body, user.FullName);
                                //await _emailSender.SendEmailAsync(message, _emailIconPath);
                            }
                        }
                        // Оцениваемым сотрудникам 10 и 13 числа месяца, следующего за месяцем оценки
                        // Ознакомиться с результатами оценки
                        if ((today.Day == 10 || today.Day == 13) && user.SystemRoles.Contains(SystemRoles.Employee))
                        {
                            var thisMonthPendingGrade = user.Grades
                                .FirstOrDefault(x =>
                                    x.DateOfCreation.AddMonths(1).Month == today.Month &&
                                    x.DateOfCreation.Year == today.Year &&
                                    x.SystemStatus == SystemStatuses.PENDING);

                            // Есть назначенные оценки в прошлом месяце
                            if (thisMonthPendingGrade != null)
                            {
                                Mail? mail = null;

                                if (today.Day == 10)
                                {
                                    mail = mails.FirstOrDefault(x => x.Code == MailCodes.CreateApprovementByEmployeeNotification);
                                }
                                else if (today.Day == 13)
                                {
                                    mail = mails.FirstOrDefault(x => x.Code == MailCodes.CreateApprovementByEmployeeReminder);
                                }

                                if (mail is null)
                                {
                                    var addressee = new string[] { "ebaturel@mtb.minsk.by" };
                                    var messageBody = "Не найдено сообщение для отправки оцениваемым сотрудникам 10 или 13 числа месяца, следующего за месяцем оценки";
                                    var errorMessage = new Message(addressee, "Ошибка импорта", messageBody, "Батурель Евгений Дмитриевич");
                                    await _emailSender.SendEmailAsync(errorMessage);
                                    _logger.Error(messageBody);
                                }

                                var message = new Message(new string[] { user.Email }, mail.Title, mail.Body, user.FullName);
                                //await _emailSender.SendEmailAsync(message, _emailIconPath);
                            }
                        }
                        // УРП 15 числа месяца, следующего за месяцем оценки
                        // Выгрузить результаты оценки
                        if (today.Day == 15 && user.SystemRoles.Contains(SystemRoles.Urp))
                        {
                            var usersWithPreviousMonthPendingGrade = users
                                .Where(x => x.Grades
                                    .Any(x =>
                                        x.DateOfCreation.AddMonths(1).Month == today.Month &&
                                        x.DateOfCreation.Year == today.Year &&
                                        x.SystemStatus == SystemStatuses.PENDING));

                            // Есть сотрудники с назначенными оценками в прошлом месяце
                            if (usersWithPreviousMonthPendingGrade.Any())
                            {
                                var mail = mails.FirstOrDefault(x => x.Code == MailCodes.CreateReportAndCheckAssessmentCompletionNotification);
                                if (mail is null)
                                {
                                    var addressee = new string[] { "ebaturel@mtb.minsk.by" };
                                    var messageBody = "Не найдено сообщение для отправки УРП 15 числа месяца, следующего за месяцем оценки";
                                    var errorMessage = new Message(addressee, "Ошибка импорта", messageBody, "Батурель Евгений Дмитриевич");
                                    await _emailSender.SendEmailAsync(errorMessage);
                                    _logger.Error(messageBody);
                                }

                                var message = new Message(new string[] { user.Email }, mail.Title, mail.Body, user.FullName);
                                //await _emailSender.SendEmailAsync(message, _emailIconPath);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        var addressee = new string[] { "ebaturel@mtb.minsk.by" };
                        var messageBody = ex.Message;
                        var errorMessage = new Message(addressee, "Ошибка импорта", messageBody, "Батурель Евгений Дмитриевич");
                        await _emailSender.SendEmailAsync(errorMessage);
                        _logger.Error(messageBody);
                    }
                }
            }
        }

        private async Task CheckUsersForGradeProcess()
        {
            using (var uow = new UnitOfWork(new ApplicationDbContext(_options)))
            {
                var employees = await uow.Users.GetAllAsync(x => x.SystemRoles.Contains(SystemRoles.Employee), includeProperties: "Grades");

                foreach (var employee in employees)
                {
                    try
                    {
                        //var lastGrade = employee.Grades.OrderByDescending(x => x.DateOfCreation).FirstOrDefault();

                        //if (lastGrade != null && ReadyForEmployeeApproval(employee, lastGrade))
                        //{
                        //    lastGrade.GradeStatus = GradeStatuses.READY_FOR_EMPLOYEE_APPROVAL;
                        //    uow.Grades.Update(lastGrade);
                        //    await uow.CommitAsync();

                        //    continue;
                        //}
                        //else if (lastGrade != null && ReadyForSupervisorEmployeeApproval(employee, lastGrade))
                        //{
                        //    lastGrade.GradeStatus = GradeStatuses.READY_FOR_SUPERVISOR_APPROVAL;
                        //    uow.Grades.Update(lastGrade);
                        //    await uow.CommitAsync();

                        //    continue;
                        //}
                        //else if (lastGrade != null &&  lastGrade.SystemStatus != SystemStatuses.PENDING)
                        //{
                        //    continue;
                        //}

                        //var currentDay = TermManager.GetDate().Day;
                        //var currentMonth = TermManager.GetDate().Month;
                        //var currentYear = TermManager.GetDate().Year;
                        //var gradeStartMonth = employee.ContractEndDate.AddMonths(-4).Month;
                        //var gradeStartYear = employee.ContractEndDate.AddMonths(-4).Year;

                        //if (currentDay != 1 || currentMonth != gradeStartMonth || currentYear != gradeStartYear)
                        //{
                        //    continue;
                        //}

                        // // // // // // TEST // // // // // //
                        int[] idArray = { 157, 176, 156, 180, 178, 195, 188, 198 };
                        if (!idArray.Contains(employee.Id))
                        {
                            continue;
                        }

                        var gradeNumber = 1;

                        if (employee.Grades.Any())
                        {
                            gradeNumber = employee.Grades.Count();
                        }

                        var newGrade = new Grade
                        {
                            Number = gradeNumber,
                            StartDate = new DateOnly(TermManager.GetDate().Year - 1, 1, 1),
                            SystemStatus = SystemStatuses.PENDING,
                            GradeStatus = GradeStatuses.PENDING,
                            UserId = employee.Id,
                            Qualification = new Qualification 
                            { 
                                CurrentJobPositionName = string.Empty,
                                EmploymentContarctTerminations = "Отсутствуют",
                                QualificationResult = $"{employee.FullName} прошел (ла) оценку соответствия (аттестацию) в Национальном Банке РБ и признана " +
                                "соответствующей квалификационным требованиям и требованиям к деловой репутации:\r\n- к должности члена коллегиального исполнительного " +
                                "органа банка, небанковской кредитно-финансовой организации от _______дата № ______ (приложение __ к пояснительной записке) действительно " +
                                "до ________г.;\r\n- к должностному лицу, выполняющему ключевые функции в банке, небанковской кредитно-финансовой организации " +
                                "от _______дата № ______ (приложение __ к пояснительной записке) действительно до ________г.;       " +
                                "*ИЛИ* \r\n\r\nНе является должностным лицом, " +
                                "выполняющим ключевые функции в ЗАО «МТБанк».\r\n"
                            },
                            ValueJudgment = new ValueJudgment(),
                            QualificationConclusion = "Руководитель соответствует квалификационным требованиям и требованиям к деловой репутации.",
                        };

                        var firstDayOfCurrentMonth = new DateOnly(TermManager.GetDate().Year, TermManager.GetDate().Month, 1);
                        var lastDayOfPreviousMonth = firstDayOfCurrentMonth.AddDays(-1);
                        newGrade.EndDate = lastDayOfPreviousMonth;

                        await uow.Grades.AddAsync(newGrade);

                        var assessmentTypes = await uow.AssessmentTypes.GetAllAsync();
                        foreach (var assessmentType in assessmentTypes)
                        {
                            var newAssessment = new Assessment
                            {
                                Number = gradeNumber,
                                SystemStatus = SystemStatuses.PENDING,
                                UserId = employee.Id,
                                AssessmentTypeId = assessmentType.Id,
                                Grade = newGrade,
                            };

                            var supervisor = await GetUserSupervisor(employee);

                            if (supervisor != null)
                            {
                                var newSupervisorAssessmentResult = new AssessmentResult
                                {
                                    SystemStatus = SystemStatuses.PENDING,
                                    JudgeId = supervisor.Id,
                                    Type = AssessmentResultTypes.SupervisorAssessment,
                                };

                                newAssessment.AssessmentResults.Add(newSupervisorAssessmentResult);
                            }

                            var newSelfAssessmentResult = new AssessmentResult
                            {
                                SystemStatus = SystemStatuses.PENDING,
                                JudgeId = employee.Id,
                                Type = AssessmentResultTypes.SelfAssessment,
                            };

                            newAssessment.AssessmentResults.Add(newSelfAssessmentResult);

                            if (assessmentType.SystemAssessmentType == SystemAssessmentTypes.СorporateСompetencies)
                            {
                                var urps = await GetAllUrpUsers();

                                foreach (var urp in urps)
                                {
                                    var newUrpAssessmentResult = new AssessmentResult
                                    {
                                        SystemStatus = SystemStatuses.PENDING,
                                        JudgeId = urp.Id,
                                        Type = AssessmentResultTypes.UrpAssessment,
                                    };

                                    newAssessment.AssessmentResults.Add(newUrpAssessmentResult);
                                }
                            }

                            await uow.Assessments.AddAsync(newAssessment);
                        }

                        await uow.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        await uow.RollbackAsync();
                        _logger.Error(ex.Message);
                        continue;
                    }
                }
            }
        }

        public bool ReadyForEmployeeApproval(User user, Grade lastGrade)
        {
            if (TermManager.GetDate().Day != 8 || !user.Grades.Any(x => x.GradeStatus == GradeStatuses.PENDING))
            {
                return false;
            }

            var nextMonthDate = lastGrade.StartDate.AddMonths(1);
            var eighthDayDate = new DateOnly(nextMonthDate.Year, nextMonthDate.Month, 8);

            return TermManager.GetDate() == eighthDayDate;
        }

        public bool ReadyForSupervisorEmployeeApproval(User user, Grade lastGrade)
        {
            if (TermManager.GetDate().Day != 11
                || !user.Grades.Any(x => x.GradeStatus == GradeStatuses.READY_FOR_EMPLOYEE_APPROVAL || x.GradeStatus == GradeStatuses.APPROVED_BY_EMPLOYEE))
            {
                return false;
            }

            var nextMonthDate = lastGrade.StartDate.AddMonths(1);
            var eleventhDayDate = new DateOnly(nextMonthDate.Year, nextMonthDate.Month, 11);

            return TermManager.GetDate() == eleventhDayDate;
        }

        private async Task<User?> GetUserSupervisor(User user)
        {
            using (var uow = new UnitOfWork(new ApplicationDbContext(_options)))
            {
                var parentSubdivision = await uow.Subdivisions.GetAsync(x => x.Id == user.ParentSubdivisionId, includeProperties: "Parent");

                if (parentSubdivision == null)
                {
                    return null;
                }

                var supervisor = await uow.Users.GetAsync(x => x.SystemRoles.Contains(SystemRoles.Supervisor) && x.SubordinateSubdivisions.Contains(parentSubdivision));

                if (supervisor != null)
                {
                    return supervisor;
                }

                var rootSubdivision = parentSubdivision.Parent;

                while (rootSubdivision != null)
                {
                    supervisor = await uow.Users.GetAsync(x => x.SystemRoles.Contains(SystemRoles.Supervisor) && x.SubordinateSubdivisions.Contains(rootSubdivision));

                    if (supervisor != null)
                    {
                        return supervisor;
                    }

                    rootSubdivision = rootSubdivision.Parent;
                }

                return null;
            }
        }

        private async Task<IEnumerable<User>> GetAllUrpUsers()
        {
            using (var uow = new UnitOfWork(new ApplicationDbContext(_options)))
            {
                var allUrpUsers = await uow.Users.GetAllAsync(x => x.SystemRoles.Contains(SystemRoles.Urp));

                return allUrpUsers;
            }
        }

        private async Task BlockNonActiveUsers(List<User> usersFromExcel)
        {
            using (var uow = new UnitOfWork(new ApplicationDbContext(_options)))
            {
                var allDbUsers = await uow.Users.GetAllAsync();

                foreach (var dbUser in allDbUsers)
                {
                    if (!usersFromExcel.Any(x => x.ServiceNumber == dbUser.ServiceNumber))
                    {
                        var nonActiveUser = await uow.Users.GetAsync(x => x.Id == dbUser.Id);
                        nonActiveUser.IsSuspended = true;

                        uow.Users.Update(nonActiveUser);
                        await uow.CommitAsync();
                    }
                }
            }
        }

        private bool IsMeetUserStructureBusinessRequirements(DataRow dataRow)
        {
            var structureRole = Convert.ToString(dataRow[46]);
            var gradeGroup = Convert.ToString(dataRow[44]);

            if (structureRole != "Начальник ССП")
            {
                return false;
            }
            else if (gradeGroup != "BOARD-1" && gradeGroup != "BOARD-2")
            {
                return false;
            }

            return true;
        }

        private bool IsMeetUserStructureBusinessRequirements(string structureRole, string gradeGroup)
        {
            if (structureRole != "Начальник ССП")
            {
                return false;
            }
            else if (gradeGroup != "BOARD-1" && gradeGroup != "BOARD-2")
            {
                return false;
            }

            return true;
        }

        private bool IsMeetUserInfoBusinessRequirements(DataRow dataRow, User userFromExcel)
        {
            var structureStatus = Convert.ToString(dataRow[27]);

            if (structureStatus != "Работник" && structureStatus != "Внеш. совместитель" && structureStatus != "Сум. учет. врем. работника")
            {
                return false;
            }

            return true;
        }

        private bool IsAdditionalUser(int userServiceNumber)
        {
            return additionalUsersServiceNumbers.Contains(userServiceNumber);
        }

        private bool IsAdditionalSubdivision(string subdivision)
        {
            return subdivision.Contains("УРП ") || subdivision.Contains("УМСТ ");
        }

        private List<int> ReadAdditionalUsersServiceNumbersFromFile(string filePath)
        {
            var numbers = new List<int>();

            try
            {
                var lines = File.ReadAllLines(filePath);

                foreach (string line in lines)
                {
                    if (int.TryParse(line, out int number))
                    {
                        numbers.Add(number);
                    }
                    else
                    {
                        _logger.Error($"Невозможно преобразовать строку в число: {line}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Ошибка при чтении файла: {ex.Message}");
            }

            return numbers;
        }

        public List<User> ReadUsersStructuresFromFile(string filePath)
        {
            var usersFromExcel = new List<User>();

            using (var fStream = File.Open(_usersStructuresPath, FileMode.Open, FileAccess.Read))
            {
                var excelDataReader = ExcelReaderFactory.CreateOpenXmlReader(fStream);
                var resultDataSet = excelDataReader.AsDataSet();
                var table = resultDataSet.Tables[0];

                for (int rowCounter = 2; rowCounter < table.Rows.Count; rowCounter++)
                {
                    try
                    {
                        var userServiceNumber = Convert.ToInt32(table.Rows[rowCounter][43]);
                        bool meetsBusinessRequirements = IsMeetUserStructureBusinessRequirements(table.Rows[rowCounter]);
                        bool isAdditionalUser = IsAdditionalUser(userServiceNumber);

                        if (!meetsBusinessRequirements && !isAdditionalUser)
                        {
                            continue;
                        }

                        var userFromExcel = new User
                        {
                            ServiceNumber = userServiceNumber,
                            GradeGroup = Convert.ToString(table.Rows[rowCounter][44]),
                            StructureRole = Convert.ToString(table.Rows[rowCounter][46]),
                        };

                        if (meetsBusinessRequirements)
                        {
                            userFromExcel.SystemRoles.Add(SystemRoles.Employee);
                        }

                        usersFromExcel.Add(userFromExcel);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex.Message);
                        continue;
                    }
                }

                excelDataReader.Close();
            }

            return usersFromExcel;
        }

        public List<User> ReadUsersInfosFromFile(string filePath, List<User> usersFromExcel)
        {
            using (var fStream = File.Open(_usersInfosPath, FileMode.Open, FileAccess.Read))
            {
                var colsArrayDocument = new XmlDocument();
                var usersPassContainerDocument = new XmlDocument();
                var excelDataReader = ExcelReaderFactory.CreateOpenXmlReader(fStream);
                var resultDataSet = excelDataReader.AsDataSet();
                var table = resultDataSet.Tables[0];

                colsArrayDocument.Load(_colsArrayPath);
                usersPassContainerDocument.Load(_usersPassContainerPath);

                for (int rowCounter = 2; rowCounter < table.Rows.Count; rowCounter++)
                {
                    try
                    {
                        var serviceNumber = Convert.ToInt32(table.Rows[rowCounter][2]);
                        var subdivision = Convert.ToString(table.Rows[rowCounter][22]);
                        var userFromExcel = usersFromExcel.FirstOrDefault(x => x.ServiceNumber == serviceNumber);

                        if (IsAdditionalSubdivision(subdivision) && userFromExcel is null)
                        {
                            userFromExcel = new User
                            {
                                ServiceNumber = serviceNumber,
                                GradeGroup = "-",
                            };

                            usersFromExcel.Add(userFromExcel);
                        }

                        if (userFromExcel is null)
                        {
                            _logger.Warn($"Не удалось найти сопоставление из СЧ в ШР для пользователя с табельным номером {serviceNumber}");
                            continue;
                        }

                        bool meetsBusinessRequirements = IsMeetUserInfoBusinessRequirements(table.Rows[rowCounter], userFromExcel);
                        bool isAdditionalUser = IsAdditionalUser(serviceNumber);
                        bool isAdditionalSubdivision = IsAdditionalSubdivision(subdivision);

                        if (!meetsBusinessRequirements)
                        {
                            if (isAdditionalUser || isAdditionalSubdivision)
                            {
                                userFromExcel.SystemRoles.Remove(SystemRoles.Employee);
                            }
                            else
                            {
                                usersFromExcel.Remove(userFromExcel);
                            }
                        }

                        userFromExcel.FullName = Convert.ToString(table.Rows[rowCounter][3]);
                        userFromExcel.Position = Convert.ToString(table.Rows[rowCounter][6]);
                        userFromExcel.HireDate = DateOnly.FromDateTime(Convert.ToDateTime(table.Rows[rowCounter][12]));
                        userFromExcel.SubdivisionFromFile = Convert.ToString(table.Rows[rowCounter][22]);
                        userFromExcel.ContractStartDate = DateOnly.FromDateTime(Convert.ToDateTime(table.Rows[rowCounter][57]));
                        userFromExcel.ContractEndDate = DateOnly.FromDateTime(Convert.ToDateTime(table.Rows[rowCounter][58]));

                        PopulateUserWithXmlData(userFromExcel, colsArrayDocument, usersPassContainerDocument);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex.Message);
                        continue;
                    }
                }

                excelDataReader.Close();
            }

            return usersFromExcel;
        }

        public async Task ProcessTrainingEvents(string filePath)
        {
            using (var uow = new UnitOfWork(new ApplicationDbContext(_options)))
            {
                var allDbUsers = await uow.Users.GetAllAsync(includeProperties: "Grades.TrainingEvents");
                var trainingEventsFromFile = new List<TrainingEvent>();

                using (var fStream = File.Open(_trainingEventsPath, FileMode.Open, FileAccess.Read))
                {
                    var excelDataReader = ExcelReaderFactory.CreateOpenXmlReader(fStream);
                    var resultDataSet = excelDataReader.AsDataSet();
                    var table = resultDataSet.Tables[0];

                    for (int rowCounter = 1; rowCounter < table.Rows.Count; rowCounter++)
                    {
                        try
                        {
                            var userFullName = Convert.ToString(table.Rows[rowCounter][0]);
                            var trainingEventFromFileName = Convert.ToString(table.Rows[rowCounter][3]);
                            var trainingEventFromFileStatus = Convert.ToString(table.Rows[rowCounter][6]);
                            var trainingEventFromFileStartDate = DateOnly.FromDateTime(Convert.ToDateTime(table.Rows[rowCounter][4]));
                            var trainingEventFromFileEndDate = DateOnly.FromDateTime(Convert.ToDateTime(table.Rows[rowCounter][5]));
                            var trainingEventFromFileCompetence = Convert.ToString(table.Rows[rowCounter][7]);

                            var dbUser = await uow.Users.GetAsync(x => x.FullName.Replace(" ", "").ToLower() == userFullName.Replace(" ", "").ToLower());

                            if (dbUser == null)
                            {
                                continue;
                            }

                            var userLastGrade = dbUser.Grades.OrderByDescending(x => x.Number).FirstOrDefault();

                            if (userLastGrade == null)
                            {
                                continue;
                            }

                            var dbTrainingEvent = userLastGrade.TrainingEvents.FirstOrDefault(x => x.Name.Replace(" ", "").ToLower() == trainingEventFromFileName.Replace(" ", "").ToLower());

                            if (dbTrainingEvent != null)
                            {
                                continue;
                            }

                            userLastGrade.TrainingEvents.Add(new TrainingEvent
                            {
                                Name = trainingEventFromFileName,
                                Status = trainingEventFromFileStatus,
                                StartDate = trainingEventFromFileStartDate,
                                EndDate = trainingEventFromFileEndDate,
                                Competence = trainingEventFromFileCompetence,
                            });

                            uow.Grades.Update(userLastGrade);
                            await uow.CommitAsync();
                        }
                        catch (Exception ex)
                        {
                            await uow.RollbackAsync();
                            _logger.Error(ex.Message);
                            continue;
                        }
                    }

                    excelDataReader.Close();
                }
            }
        }

        private async Task<List<User>> GetSubordinateUsers(int supervisorId)
        {
            using (var uow = new UnitOfWork(new ApplicationDbContext(_options)))
            {
                var supervisor = await uow.Users.GetAsync(x => x.Id == supervisorId, includeProperties: new string[]
                {
                    "SubordinateSubdivisions.Users.Grades",
                    "SubordinateSubdivisions.Children.Users.Grades",
                });

                if (supervisor == null)
                {
                    // Обработка случая, когда руководитель не найден
                    return new List<User>(); // или выбросьте исключение
                }

                var allSubordinateUsers = new List<User>();

                foreach (var subdivision in supervisor.SubordinateSubdivisions)
                {
                    var subordinateUsersFromSubdivision = await GetSubordinateUsers(subdivision);
                    allSubordinateUsers.AddRange(subordinateUsersFromSubdivision);
                }

                return allSubordinateUsers;
            }
        }

        private async Task<List<User>> GetSubordinateUsers(Subdivision subdivision)
        {
            using (var uow = new UnitOfWork(new ApplicationDbContext(_options)))
            {
                var subordinateUsers = new List<User>();

                // Получаем пользователей с ролью "Сотрудник"
                subordinateUsers.AddRange(subdivision.Users
                    .Where(x => x.SystemRoles.Contains(SystemRoles.Employee)));

                // Рекурсивно получаем пользователей из дочерних подразделений
                foreach (var childSubdivision in subdivision.Children)
                {
                    var subordinateUsersFromChildSubdivision = await GetSubordinateUsers(childSubdivision);
                    subordinateUsers.AddRange(subordinateUsersFromChildSubdivision);
                }

                return subordinateUsers;
            }
        }

        private async Task<User?> GetSupervisorForUser(int userId)
        {
            using (var uow = new UnitOfWork(new ApplicationDbContext(_options)))
            {
                var user = await uow.Users.GetAsync(x => x.Id == userId);
                var parentSubdivision = await uow.Subdivisions.GetAsync(x => x.Id == user.ParentSubdivisionId, includeProperties: "Parent");

                if (parentSubdivision == null)
                {
                    return null;
                }

                var supervisor = await uow.Users.GetAsync(x => x.SystemRoles.Contains(SystemRoles.Supervisor) && x.SubordinateSubdivisions.Contains(parentSubdivision));
                if (supervisor != null)
                {
                    return supervisor;
                }

                var rootSubdivision = parentSubdivision.Parent;

                while (rootSubdivision != null)
                {
                    supervisor = await uow.Users.GetAsync(x => x.SystemRoles.Contains(SystemRoles.Supervisor) && x.SubordinateSubdivisions.Contains(rootSubdivision));

                    if (supervisor != null)
                    {
                        return supervisor;
                    }

                    rootSubdivision = rootSubdivision.Parent;
                }

                return null;
            }
        }
    }
}