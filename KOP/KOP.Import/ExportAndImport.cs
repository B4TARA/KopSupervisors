using System.Data;
using System.Xml;
using ExcelDataReader;
using KOP.BLL.Validators;
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
            var emailConfiguration = new EmailConfiguration
            {
                From = "KOP Sender",
                SmtpServer = "LDGate.mtb.minsk.by",
                Port = 25,
            };

            _options = options;
            _config = config;
            _emailSender = new EmailSender(emailConfiguration);

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

                var usersFromExcel = ProcessUsers();
                await PutUsersInDatabase(usersFromExcel);
                await BlockNonActiveUsers(usersFromExcel);
                await CheckUsersForGradeProcess();
                //await CheckForNotifications();
                await ProcessTrainingEvents(_trainingEventsPath);
            }
            catch (Exception ex)
            {
                // Отправлять E-mail себе на почту
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
            if(IsAdditionalSubdivision(userFromExcel.SubdivisionFromFile))
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

                var email = (string)obj["email"];
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
            //userFromExcel.SystemRoles.Add(SystemRoles.Employee);

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
                var users = await uow.Users.GetAllAsync(x => x.SystemRoles.Contains(SystemRoles.Employee));
                var mails = await uow.Mails.GetAllAsync();
                var pendingAssessmentResults = await uow.AssessmentResults.GetAllAsync(x => x.SystemStatus == SystemStatuses.PENDING);

                foreach (var user in users)
                {
                    try
                    {
                        if (today.Day != 1 && today.Day != 15)
                        {
                            continue;
                        }

                        if (user.SystemRoles.Contains(SystemRoles.Supervisor) && today.Day == 1)
                        {
                            var mail = mails.FirstOrDefault(x => x.Code == MailCodes.CreateAssessmentGroupAndAssessEmployeeNotification);

                            if (mail is null)
                            {
                                throw new Exception("Не найдено сообщение для отправки непосредственному руковолителю 1 числа");
                            }

                            await _emailSender.SendEmailAsync(new Message(new string[] { user.Email }, mail.Title, mail.Body, user.FullName), _emailIconPath);
                        }
                        else if (user.SystemRoles.Contains(SystemRoles.Supervisor) && today.Day == 15)
                        {
                            var mail = mails.FirstOrDefault(x => x.Code == MailCodes.CreateAssessmentGroupAndAssessEmployeeReminder);

                            if (mail is null)
                            {
                                throw new Exception("Не найдено сообщение для отправки непосредственному руковолителю 15 числа");
                            }

                            await _emailSender.SendEmailAsync(new Message(new string[] { user.Email }, mail.Title, mail.Body, user.FullName), _emailIconPath);
                        }

                        if (user.SystemRoles.Intersect(new List<SystemRoles> { SystemRoles.Umst, SystemRoles.Cup, SystemRoles.Urp }).Any() && today.Day == 1)
                        {
                            var mail = mails.FirstOrDefault(x => x.Code == MailCodes.CreatePerformanceResultsAndSelfAssessmentNotification);

                            if (mail is null)
                            {
                                throw new Exception("Не найдено сообщение для отправки оцениваемому сотруднику 1 числа");
                            }

                            await _emailSender.SendEmailAsync(new Message(new string[] { user.Email }, mail.Title, mail.Body, user.FullName), _emailIconPath);
                        }
                        else if (user.SystemRoles.Intersect(new List<SystemRoles> { SystemRoles.Umst, SystemRoles.Cup, SystemRoles.Urp }).Any() && today.Day == 15)
                        {
                            var mail = mails.FirstOrDefault(x => x.Code == MailCodes.CreatePerformanceResultsAndSelfAssessmentReminder);

                            if (mail is null)
                            {
                                throw new Exception("Не найдено сообщение для отправки оцениваемому сотруднику 15 числа");
                            }

                            await _emailSender.SendEmailAsync(new Message(new string[] { user.Email }, mail.Title, mail.Body, user.FullName), _emailIconPath);
                        }

                        if (user.SystemRoles.Contains(SystemRoles.Employee) && today.Day == 1)
                        {
                            var mail = mails.FirstOrDefault(x => x.Code == MailCodes.CreateCriteriaNotification);

                            if (mail is null)
                            {
                                throw new Exception("Не найдено сообщение для отправки ответственным за заполнение показателей 1 числа");
                            }

                            await _emailSender.SendEmailAsync(new Message(new string[] { user.Email }, mail.Title, mail.Body, user.FullName), _emailIconPath);
                        }
                        else if (user.SystemRoles.Contains(SystemRoles.Employee) && today.Day == 15)
                        {
                            var mail = mails.FirstOrDefault(x => x.Code == MailCodes.CreateCriteriaReminder);

                            if (mail is null)
                            {
                                throw new Exception("Не найдено сообщение для отправки ответственным за заполнение показателей 15 числа");
                            }

                            await _emailSender.SendEmailAsync(new Message(new string[] { user.Email }, mail.Title, mail.Body, user.FullName), _emailIconPath);
                        }

                        if (pendingAssessmentResults.Any(x => x.JudgeId == user.Id) && today.Day == 10)
                        {
                            var mail = mails.FirstOrDefault(x => x.Code == MailCodes.CreateAssessmentNotification);

                            if (mail is null)
                            {
                                throw new Exception("Не найдено сообщение для отправки группе оценки КК 1 числа");
                            }

                            await _emailSender.SendEmailAsync(new Message(new string[] { user.Email }, mail.Title, mail.Body, user.FullName), _emailIconPath);
                        }
                        else if (pendingAssessmentResults.Any(x => x.JudgeId == user.Id) && today.Day == 20)
                        {
                            var mail = mails.FirstOrDefault(x => x.Code == MailCodes.CreateAssessmentReminder);

                            if (mail is null)
                            {
                                throw new Exception("Не найдено сообщение для отправки группе оценки КК 15 числа");
                            }

                            await _emailSender.SendEmailAsync(new Message(new string[] { user.Email }, mail.Title, mail.Body, user.FullName), _emailIconPath);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"Произошла ошибка во время работы с уведомлением пользователя {user.FullName} : {ex.Message}");
                    }
                }
            }
        }

        private async Task CheckUsersForGradeProcess()
        {
            using (var uow = new UnitOfWork(new ApplicationDbContext(_options)))
            {
                var employees = await uow.Users.GetAllAsync(x => x.SystemRoles.Contains(SystemRoles.Employee) && !x.Grades.Any(x => x.SystemStatus == SystemStatuses.PENDING), includeProperties: "Grades");

                foreach (var employee in employees)
                {
                    try
                    {
                        var currentDay = TermManager.GetDate().Day;
                        var currentMonth = TermManager.GetDate().Month;
                        var currentYear = TermManager.GetDate().Year;
                        var gradeStartMonth = employee.ContractEndDate.AddMonths(-4).Month;
                        var gradeStartYear = employee.ContractEndDate.AddMonths(-4).Year;

                        if (currentDay != 1 || currentMonth != gradeStartMonth || currentYear != gradeStartYear)
                        {
                            continue;
                        }

                        var gradeStartDate = new DateOnly(currentYear, currentMonth, 1);
                        var gradeNumber = 1;

                        if (employee.Grades.Any())
                        {
                            gradeNumber = employee.Grades.Count();
                        }

                        var newGrade = new Grade
                        {
                            Number = gradeNumber,
                            StartDate = new DateOnly(TermManager.GetDate().Year - 1, 1, 1),
                            EndDate = new DateOnly(TermManager.GetDate().Year, 12, 31),
                            SystemStatus = SystemStatuses.PENDING,
                            UserId = employee.Id,
                            Qualification = new Qualification { SupervisorSspName = employee.FullName },
                            ValueJudgment = new ValueJudgment(),
                            QualificationConclusion = "Руководитель соответствует квалификационным требованиям и требованиям к деловой репутации.",
                        };

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
                            };

                            var supervisor = await GetUserSupervisor(employee);

                            if (supervisor != null)
                            {
                                var newSupervisorAssessmentResult = new AssessmentResult
                                {
                                    SystemStatus = SystemStatuses.PENDING,
                                    JudgeId = supervisor.Id,
                                };

                                newAssessment.AssessmentResults.Add(newSupervisorAssessmentResult);
                            }

                            var newSelfAssessmentResult = new AssessmentResult
                            {
                                SystemStatus = SystemStatuses.PENDING,
                                JudgeId = employee.Id,
                            };

                            newAssessment.AssessmentResults.Add(newSelfAssessmentResult);

                            if (assessmentType.SystemAssessmentType == SystemAssessmentTypes.СorporateСompetencies)
                            {
                                var urps = await GetAllUrpUsers();

                                foreach (var urp in urps)
                                {
                                    var newUepAssessmentResult = new AssessmentResult
                                    {
                                        SystemStatus = SystemStatuses.PENDING,
                                        JudgeId = urp.Id,
                                    };

                                    newAssessment.AssessmentResults.Add(newUepAssessmentResult);
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
    }
}