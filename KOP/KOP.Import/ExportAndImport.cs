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
using System.Data;
using System.Xml;
using SystemRoles = KOP.Common.Enums.SystemRoles;

namespace KOP.Import
{
    public class ExportAndImport
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly IEmailSender _emailSender;
        private readonly IConfigurationRoot _config;

        private readonly string _referenceBookPath;
        private readonly string _usersInfosPath;
        private readonly string _usersStructuresPath;
        private readonly string _colsArrayPath;
        private readonly string _usersPassContainerPath;
        private readonly string _usersImgDownloadPath;
        private readonly string _emailIconPath;

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

            _referenceBookPath = _config["FilePaths:ReferenceBookPath"] ?? "";
            _usersInfosPath = _config["FilePaths:UsersInfosPath"] ?? "";
            _usersStructuresPath = _config["FilePaths:UsersStructuresPath"] ?? "";
            _colsArrayPath = _config["FilePaths:ColsArrayPath"] ?? "";
            _usersPassContainerPath = _config["FilePaths:UsersPassContainerPath"] ?? "";
            _usersImgDownloadPath = _config["FilePaths:UsersImgDownloadPath"] ?? "";
            _emailIconPath = _config["FilePaths:EmailIconPath"] ?? "";
        }

        public async Task TransferDataFromExcelToDatabase()
        {
            try
            {
                var usersFromExcel = ProcessUsers();
                await PutUsersInDatabase(usersFromExcel);
                await BlockNonActiveUsers(usersFromExcel);
                await CheckUsersForGradeProcess();
                //await CheckForNotifications();
            }
            catch (Exception ex)
            {
                // Отправлять E-mail себе на почту
                _logger.Error(ex.Message);
            }
        }

        private List<User> ProcessUsers()
        {
            var usersFromExcel = new List<User>();

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (var fStream = File.Open(_usersStructuresPath, FileMode.Open, FileAccess.Read))
            {
                var excelDataReader = ExcelReaderFactory.CreateOpenXmlReader(fStream);
                var resultDataSet = excelDataReader.AsDataSet();
                var table = resultDataSet.Tables[0];

                for (int rowCounter = 2; rowCounter < table.Rows.Count; rowCounter++)
                {
                    try
                    {
                        if (!IsMeetUserStructureBusinessRequirements(table.Rows[rowCounter]))
                        {
                            continue;
                        }

                        var userFromExcel = new User
                        {
                            ServiceNumber = Convert.ToInt32(table.Rows[rowCounter][43]),
                            GradeGroup = Convert.ToString(table.Rows[rowCounter][44]),
                        };

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
                        var userFromExcel = usersFromExcel.FirstOrDefault(x => x.ServiceNumber == serviceNumber);

                        if (userFromExcel is null)
                        {
                            _logger.Warn($"Не удалось найти сопоставление из СЧ в ШР для пользователя с табельным номером {serviceNumber}");
                            continue;
                        }
                        else if (!IsMeetUserInfoBusinessRequirements(table.Rows[rowCounter]))
                        {
                            usersFromExcel.Remove(userFromExcel);
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

        private async Task PopulateUserWithModule(User userFromExcel, UnitOfWork uow, string parentSubdivisionName)
        {
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
            if (userFromExcel.SubdivisionFromFile is null)
            {
                return;
            }
            else if (userFromExcel.SubdivisionFromFile.Contains("УМСТ"))
            {
                userFromExcel.SystemRoles.Add(SystemRoles.Umst);

                var rootSubdivisions = await uow.Subdivisions.GetAllAsync(x => x.NestingLevel == 1);

                userFromExcel.SubordinateSubdivisions = rootSubdivisions.ToList();
            }
            else if (userFromExcel.SubdivisionFromFile.Contains("ЦУП"))
            {
                userFromExcel.SystemRoles.Add(SystemRoles.Cup);

                var rootSubdivisions = await uow.Subdivisions.GetAllAsync(x => x.NestingLevel == 1);

                userFromExcel.SubordinateSubdivisions = rootSubdivisions.ToList();
            }
            else if (userFromExcel.SubdivisionFromFile.Contains("УРП"))
            {
                userFromExcel.SystemRoles.Add(SystemRoles.Urp);

                var rootSubdivisions = await uow.Subdivisions.GetAllAsync(x => x.NestingLevel == 1);

                userFromExcel.SubordinateSubdivisions = rootSubdivisions.ToList();
            }
            else if (userFromExcel.SubdivisionFromFile.Contains("УОП"))
            {
                userFromExcel.SystemRoles.Add(SystemRoles.Uop);

                var rootSubdivisions = await uow.Subdivisions.GetAllAsync(x => x.NestingLevel == 1);

                userFromExcel.SubordinateSubdivisions = rootSubdivisions.ToList();
            }
            else
            {
                userFromExcel.SystemRoles.Add(SystemRoles.Employee);
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
                        existingUser.SystemRoles = userFromExcel.SystemRoles;

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
                var today = TermManager.GetDate();
                var users = await uow.Users.GetAllAsync(x => x.SystemRoles.Contains(SystemRoles.Employee));

                foreach (var user in users)
                {
                    try
                    {
                        var currentDay = today.Day;
                        var currentMonth = today.Month;
                        var currentYear = today.Year;
                        var gradeStartMonth = user.ContractEndDate.AddMonths(-4).Month;
                        var gradeStartYear = user.ContractEndDate.AddMonths(-4).Year;

                        if (currentDay != 1 || currentMonth != gradeStartMonth || currentYear != gradeStartYear)
                        {
                            continue;
                        }

                        var gradeStartDate = new DateOnly(currentYear, currentMonth, 1);

                        var userGrades = await uow.Grades.GetAllAsync(x => x.UserId == user.Id);
                        var gradeNumber = 1;

                        if (userGrades.Any())
                        {
                            gradeNumber = userGrades.Count();
                        }

                        var newGrade = new Grade
                        {
                            Number = gradeNumber,
                            StartDate = today.AddYears(-2),
                            EndDate = today.AddMonths(-1),
                            SystemStatus = SystemStatuses.PENDING,
                            UserId = user.Id,
                            Qualification = new Qualification(),
                        };

                        await uow.Grades.AddAsync(newGrade);

                        var assessmentTypes = await uow.AssessmentTypes.GetAllAsync(includeProperties: "Assessments");

                        foreach (var assessmentType in assessmentTypes)
                        {
                            var assessmentNumber = 1;

                            if (assessmentType.Assessments.Any())
                            {
                                gradeNumber = userGrades.Count();
                            }

                            var newAssessment = new Assessment
                            {
                                Number = assessmentNumber,
                                SystemStatus = SystemStatuses.PENDING,
                                UserId = user.Id,
                                AssessmentTypeId = assessmentType.Id,
                            };

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

        private bool IsMeetUserInfoBusinessRequirements(DataRow dataRow)
        {
            var structureStatus = Convert.ToString(dataRow[27]);

            if (structureStatus != "Работник" && structureStatus != "Внеш. совместитель" && structureStatus != "Сум. учет. врем. работника")
            {
                return false;
            }

            return true;
        }
    }
}