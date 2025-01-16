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
                await ProcessModules();
                var employeesFromExcel = ProcessEmployees();
                await UpdateOrCreateUsersInDatabase(employeesFromExcel);
                await BlockNonActiveUsers(employeesFromExcel);
                await CheckEmployeesForGradeProcess();
                //await CheckForNotifications();
            }
            catch (Exception ex)
            {
                // Отправлять E-mail себе на почту
                _logger.Error(ex.Message);
            }
        }

        private List<Employee> ProcessEmployees()
        {
            var employeesFromExcel = new List<Employee>();

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

                        var employeeFromExcel = new Employee
                        {
                            ServiceNumber = Convert.ToInt32(table.Rows[rowCounter][43]),
                            GradeGroup = Convert.ToString(table.Rows[rowCounter][44]),
                        };

                        employeesFromExcel.Add(employeeFromExcel);
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
                        var employeeFromExcel = employeesFromExcel.FirstOrDefault(x => x.ServiceNumber == serviceNumber);

                        if (employeeFromExcel is null)
                        {
                            _logger.Warn($"Не удалось найти сопоставление из СЧ в ШР для пользователя с табельным номером {serviceNumber}");
                            continue;
                        }
                        else if (!IsMeetUserInfoBusinessRequirements(table.Rows[rowCounter]))
                        {
                            employeesFromExcel.Remove(employeeFromExcel);
                        }

                        employeeFromExcel.FullName = Convert.ToString(table.Rows[rowCounter][3]);
                        employeeFromExcel.Position = Convert.ToString(table.Rows[rowCounter][6]);
                        employeeFromExcel.HireDate = DateOnly.FromDateTime(Convert.ToDateTime(table.Rows[rowCounter][12]));
                        employeeFromExcel.Subdivision = Convert.ToString(table.Rows[rowCounter][22]);
                        employeeFromExcel.ContractStartDate = DateOnly.FromDateTime(Convert.ToDateTime(table.Rows[rowCounter][57]));
                        employeeFromExcel.ContractEndDate = DateOnly.FromDateTime(Convert.ToDateTime(table.Rows[rowCounter][58]));

                        PopulateEmployeeWithXmlData(employeeFromExcel, colsArrayDocument, usersPassContainerDocument);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex.Message);
                        continue;
                    }
                }

                excelDataReader.Close();
            }

            return employeesFromExcel;
        }

        private async Task ProcessModules()
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (var fStream = File.Open(_referenceBookPath, FileMode.Open, FileAccess.Read))
            {
                var excelDataReader = ExcelReaderFactory.CreateOpenXmlReader(fStream);
                var resultDataSet = excelDataReader.AsDataSet();
                var table = resultDataSet.Tables[0];

                var modules = new List<Module>();
                var excelRows = new List<ExcelRow>();

                for (int rowCounter = 2; rowCounter < table.Rows.Count; rowCounter++)
                {
                    try
                    {
                        var moduleName = Convert.ToString(table.Rows[rowCounter][0]);
                        var parentName = Convert.ToString(table.Rows[rowCounter][3]);
                        var curatorServiceNumber = Convert.ToInt32(table.Rows[rowCounter][4].ToString());
                        var supervisorServiceNumber = Convert.ToInt32(table.Rows[rowCounter][6].ToString());

                        if (string.IsNullOrEmpty(moduleName) || string.IsNullOrEmpty(parentName))
                        {
                            continue;
                        }

                        excelRows.Add(new ExcelRow
                        {
                            ModuleName = moduleName,
                            ParentModuleName = parentName,
                            CuratorServiceNumber = curatorServiceNumber,
                            SupervisorServiceNumber = supervisorServiceNumber,
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex.Message);
                        continue;
                    }
                }

                excelDataReader.Close();

                var rootModulesFromExcel = excelRows.GroupBy(x => x.ParentModuleName);
                var curatorsFromExcel = excelRows.GroupBy(x => x.CuratorServiceNumber);
                var supervisorsFromExcel = excelRows.GroupBy(x => x.SupervisorServiceNumber);

                using (var uow = new UnitOfWork(new ApplicationDbContext(_options)))
                {
                    foreach (var rootModuleFromExcel in rootModulesFromExcel)
                    {
                        try
                        {
                            var newRootModule = new Module
                            {
                                Name = rootModuleFromExcel.Key,
                            };

                            foreach (var childModuleFromExcel in rootModuleFromExcel)
                            {
                                newRootModule.Children.Add(new Module
                                {
                                    Name = childModuleFromExcel.ModuleName,
                                    Parent = newRootModule,
                                });
                            }

                            await uow.Modules.AddAsync(newRootModule);
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(ex.Message);
                            continue;
                        }
                    }

                    await uow.CommitAsync();
                }

                using (var uow = new UnitOfWork(new ApplicationDbContext(_options)))
                {
                    foreach (var curatorFromExcel in curatorsFromExcel)
                    {
                        try
                        {
                            var newCuratorEmployee = new Employee
                            {
                                Name = rootModuleFromExcel.Key,
                            };

                            foreach (var childModuleFromExcel in rootModuleFromExcel)
                            {
                                newRootModule.Children.Add(new Module
                                {
                                    Name = childModuleFromExcel.ModuleName,
                                    Parent = newRootModule,
                                });
                            }

                            await uow.Modules.AddAsync(newRootModule);
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(ex.Message);
                            continue;
                        }
                    }

                    await uow.CommitAsync();
                }

                using (var uow = new UnitOfWork(new ApplicationDbContext(_options)))
                {
                    foreach (var rootModuleFromExcel in rootModulesFromExcel)
                    {
                        try
                        {
                            var newRootModule = new Module
                            {
                                Name = rootModuleFromExcel.Key,
                            };

                            foreach (var childModuleFromExcel in rootModuleFromExcel)
                            {
                                newRootModule.Children.Add(new Module
                                {
                                    Name = childModuleFromExcel.ModuleName,
                                    Parent = newRootModule,
                                });
                            }

                            await uow.Modules.AddAsync(newRootModule);
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(ex.Message);
                            continue;
                        }
                    }

                    await uow.CommitAsync();
                }
            }
        }

        private async Task PopulateEmployeeWithModule(Employee employeeFromExcel, UnitOfWork uow)
        {
            var subdivision = employeeFromExcel.Subdivision;

            if (subdivision is null)
            {
                return;
            }

            var module = await uow.Modules.GetAsync(x => x.Name.Replace(" ", "").ToLower() == subdivision.Replace(" ", "").ToLower());

            if (module is null)
            {
                return;
            }

            employeeFromExcel.Modules.Add(module);
        }

        private void PopulateEmployeeWithXmlData(Employee employeeFromExcel, XmlDocument colsArrayDocument, XmlDocument usersPassContainerDocument)
        {
            if (employeeFromExcel.FullName is null)
            {
                return;
            }

            var parts = employeeFromExcel.FullName.Split(' ');
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
                var fileName = employeeFromExcel.FullName.Replace(" ", "");
                var imagePath = ImageUtilities.SaveBase64Image(base64Image, fileName, _usersImgDownloadPath);

                employeeFromExcel.ImagePath = imagePath;
                employeeFromExcel.Email = email;
            }

            var userNode2 = usersPassContainerDocument.SelectSingleNode(xpath);

            if (userNode1 != null && userNode2 != null)
            {
                var obj = JObject.Parse(userNode2.InnerText);

                var login = (string)obj["login"];
                var password = (string)obj["password"];

                employeeFromExcel.Login = login;
                employeeFromExcel.Password = password;
            }
        }

        private async Task PopulateEmployeeWithRole(Employee employeeFromExcel, UnitOfWork uow)
        {
            if (employeeFromExcel.Subdivision is null)
            {
                return;
            }
            else if (employeeFromExcel.Subdivision.Contains("УМСТ"))
            {
                employeeFromExcel.SystemRoles.Add(SystemRoles.Umst);

                var verticales = await uow.Modules.GetAllAsync(x => x.ModuleTypeId == 1);

                employeeFromExcel.Modules = verticales.ToList();
            }
            else if (employeeFromExcel.Subdivision.Contains("ЦУП"))
            {
                employeeFromExcel.SystemRoles.Add(SystemRoles.Cup);

                var verticales = await uow.Modules.GetAllAsync(x => x.ModuleTypeId == 1);

                employeeFromExcel.Modules = verticales.ToList();
            }
            else if (employeeFromExcel.Subdivision.Contains("УРП"))
            {
                employeeFromExcel.SystemRoles.Add(SystemRoles.Urp);

                var verticales = await uow.Modules.GetAllAsync(x => x.ModuleTypeId == 1);

                employeeFromExcel.Modules = verticales.ToList();
            }
            else if (employeeFromExcel.Subdivision.Contains("УОП"))
            {
                employeeFromExcel.SystemRoles.Add(SystemRoles.Uop);

                var verticales = await uow.Modules.GetAllAsync(x => x.ModuleTypeId == 1);

                employeeFromExcel.Modules = verticales.ToList();
            }
            else
            {
                employeeFromExcel.SystemRoles.Add(SystemRoles.Employee);
            }
        }

        private async Task UpdateOrCreateUsersInDatabase(List<Employee> employeesFromExcel)
        {
            var invalidEmployees = new List<Employee>();

            using (var uow = new UnitOfWork(new ApplicationDbContext(_options)))
            {
                var _employeeValidator = new EmployeeValidator(uow);

                foreach (var employeeFromExcel in employeesFromExcel)
                {
                    try
                    {
                        var result = _employeeValidator.Validate(employeeFromExcel);

                        if (!result.IsValid)
                        {
                            invalidEmployees.Add(employeeFromExcel);

                            continue;
                        }

                        var existingEmployee = await uow.Employees.GetAsync(x => x.ServiceNumber == employeeFromExcel.ServiceNumber, includeProperties: "Modules");

                        if (existingEmployee != null)
                        {
                            existingEmployee.FullName = employeeFromExcel.FullName;
                            existingEmployee.Position = employeeFromExcel.Position;
                            existingEmployee.Subdivision = employeeFromExcel.Subdivision;
                            existingEmployee.GradeGroup = employeeFromExcel.GradeGroup;
                            existingEmployee.HireDate = employeeFromExcel.HireDate;
                            existingEmployee.ContractStartDate = employeeFromExcel.ContractStartDate;
                            existingEmployee.ContractEndDate = employeeFromExcel.ContractEndDate;
                            existingEmployee.Login = employeeFromExcel.Login;
                            existingEmployee.Password = employeeFromExcel.Password;
                            existingEmployee.Email = employeeFromExcel.Email;
                            existingEmployee.ImagePath = employeeFromExcel.ImagePath;

                            uow.Employees.Update(existingEmployee);
                        }
                        else
                        {
                            await PopulateEmployeeWithRole(employeeFromExcel, uow);
                            await PopulateEmployeeWithModule(employeeFromExcel, uow);

                            await uow.Employees.AddAsync(employeeFromExcel);
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

        private async Task CheckForNotifications()
        {
            using (var uow = new UnitOfWork(new ApplicationDbContext(_options)))
            {
                var today = TermManager.GetDate();
                var employees = await uow.Employees.GetAllAsync(x => x.SystemRoles.Contains(SystemRoles.Employee));
                var mails = await uow.Mails.GetAllAsync();
                var pendingAssessmentResults = await uow.AssessmentResults.GetAllAsync(x => x.SystemStatus == SystemStatuses.PENDING);

                foreach (var employee in employees)
                {
                    try
                    {
                        if (today.Day != 1 && today.Day != 15)
                        {
                            continue;
                        }

                        if (employee.SystemRoles.Contains(SystemRoles.Supervisor) && today.Day == 1)
                        {
                            var mail = mails.FirstOrDefault(x => x.Code == MailCodes.CreateAssessmentGroupAndAssessEmployeeNotification);

                            if (mail is null)
                            {
                                throw new Exception("Не найдено сообщение для отправки непосредственному руковолителю 1 числа");
                            }

                            await _emailSender.SendEmailAsync(new Message(new string[] { employee.Email }, mail.Title, mail.Body, employee.FullName), _emailIconPath);
                        }
                        else if(employee.SystemRoles.Contains(SystemRoles.Supervisor) && today.Day == 15)
                        {
                            var mail = mails.FirstOrDefault(x => x.Code == MailCodes.CreateAssessmentGroupAndAssessEmployeeReminder);

                            if (mail is null)
                            {
                                throw new Exception("Не найдено сообщение для отправки непосредственному руковолителю 15 числа");
                            }

                            await _emailSender.SendEmailAsync(new Message(new string[] { employee.Email }, mail.Title, mail.Body, employee.FullName), _emailIconPath);
                        }

                        if (employee.SystemRoles.Intersect(new List<SystemRoles> { SystemRoles.Umst, SystemRoles.Cup, SystemRoles.Urp }).Any() && today.Day == 1)
                        {
                            var mail = mails.FirstOrDefault(x => x.Code == MailCodes.CreatePerformanceResultsAndSelfAssessmentNotification);

                            if (mail is null)
                            {
                                throw new Exception("Не найдено сообщение для отправки оцениваемому сотруднику 1 числа");
                            }

                            await _emailSender.SendEmailAsync(new Message(new string[] { employee.Email }, mail.Title, mail.Body, employee.FullName), _emailIconPath);
                        }
                        else if (employee.SystemRoles.Intersect(new List<SystemRoles> { SystemRoles.Umst, SystemRoles.Cup, SystemRoles.Urp }).Any() && today.Day == 15)
                        {
                            var mail = mails.FirstOrDefault(x => x.Code == MailCodes.CreatePerformanceResultsAndSelfAssessmentReminder);

                            if (mail is null)
                            {
                                throw new Exception("Не найдено сообщение для отправки оцениваемому сотруднику 15 числа");
                            }

                            await _emailSender.SendEmailAsync(new Message(new string[] { employee.Email }, mail.Title, mail.Body, employee.FullName), _emailIconPath);
                        }

                        if (employee.SystemRoles.Contains(SystemRoles.Employee) && today.Day == 1)
                        {
                            var mail = mails.FirstOrDefault(x => x.Code == MailCodes.CreateCriteriaNotification);

                            if (mail is null)
                            {
                                throw new Exception("Не найдено сообщение для отправки ответственным за заполнение показателей 1 числа");
                            }

                            await _emailSender.SendEmailAsync(new Message(new string[] { employee.Email }, mail.Title, mail.Body, employee.FullName), _emailIconPath);
                        }
                        else if (employee.SystemRoles.Contains(SystemRoles.Employee) && today.Day == 15)
                        {
                            var mail = mails.FirstOrDefault(x => x.Code == MailCodes.CreateCriteriaReminder);

                            if (mail is null)
                            {
                                throw new Exception("Не найдено сообщение для отправки ответственным за заполнение показателей 15 числа");
                            }

                            await _emailSender.SendEmailAsync(new Message(new string[] { employee.Email }, mail.Title, mail.Body, employee.FullName), _emailIconPath);
                        }

                        if (pendingAssessmentResults.Any(x => x.JudgeId == employee.Id) && today.Day == 10)
                        {
                            var mail = mails.FirstOrDefault(x => x.Code == MailCodes.CreateAssessmentNotification);

                            if (mail is null)
                            {
                                throw new Exception("Не найдено сообщение для отправки группе оценки КК 1 числа");
                            }

                            await _emailSender.SendEmailAsync(new Message(new string[] { employee.Email }, mail.Title, mail.Body, employee.FullName), _emailIconPath);
                        }
                        else if (pendingAssessmentResults.Any(x => x.JudgeId == employee.Id) && today.Day == 20)
                        {
                            var mail = mails.FirstOrDefault(x => x.Code == MailCodes.CreateAssessmentReminder);

                            if (mail is null)
                            {
                                throw new Exception("Не найдено сообщение для отправки группе оценки КК 15 числа");
                            }

                            await _emailSender.SendEmailAsync(new Message(new string[] { employee.Email }, mail.Title, mail.Body, employee.FullName), _emailIconPath);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"Произошла ошибка во время работы с уведомлением пользователя {employee.FullName} : {ex.Message}");
                    }
                }
            }
        }

        private async Task CheckEmployeesForGradeProcess()
        {
            using (var uow = new UnitOfWork(new ApplicationDbContext(_options)))
            {
                var today = TermManager.GetDate();
                var employees = await uow.Employees.GetAllAsync(x => x.SystemRoles.Contains(SystemRoles.Employee));

                foreach (var employee in employees)
                {
                    try
                    {
                        var currentDay = today.Day;
                        var currentMonth = today.Month;
                        var currentYear = today.Year;
                        var gradeStartMonth = employee.ContractEndDate.Value.AddMonths(-4).Month;
                        var gradeStartYear = employee.ContractEndDate.Value.AddMonths(-4).Year;

                        if (currentDay != 1 || currentMonth != gradeStartMonth || currentYear != gradeStartYear)
                        {
                            continue;
                        }

                        var gradeStartDate = new DateOnly(currentYear, currentMonth, 1);

                        var employeeGrades = await uow.Grades.GetAllAsync(x => x.EmployeeId == employee.Id);
                        var gradeNumber = 1;

                        if (employeeGrades.Any())
                        {
                            gradeNumber = employeeGrades.Count();
                        }

                        var newGrade = new Grade
                        {
                            Number = gradeNumber,
                            StartDate = today.AddYears(-2),
                            EndDate = today.AddMonths(-1),
                            SystemStatus = SystemStatuses.PENDING,
                            EmployeeId = employee.Id,
                            Qualification = new Qualification(),
                        };

                        await uow.Grades.AddAsync(newGrade);

                        var assessmentTypes = await uow.AssessmentTypes.GetAllAsync(includeProperties: "Assessments");

                        foreach (var assessmentType in assessmentTypes)
                        {
                            var assessmentNumber = 1;

                            if (assessmentType.Assessments.Any())
                            {
                                gradeNumber = employeeGrades.Count();
                            }

                            var newAssessment = new Assessment
                            {
                                Number = assessmentNumber,
                                SystemStatus = SystemStatuses.PENDING,
                                EmployeeId = employee.Id,
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

        private async Task BlockNonActiveUsers(List<Employee> employeesFromExcel)
        {
            using (var uow = new UnitOfWork(new ApplicationDbContext(_options)))
            {
                var allDbUsers = await uow.Employees.GetAllAsync();

                foreach (var dbUser in allDbUsers)
                {
                    if (!employeesFromExcel.Any(x => x.ServiceNumber == dbUser.ServiceNumber))
                    {
                        var nonActiveUser = await uow.Employees.GetAsync(x => x.Id == dbUser.Id);
                        nonActiveUser.IsSuspended = true;

                        uow.Employees.Update(nonActiveUser);
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

    public class ExcelRow
    {
        public string ModuleName { get; set; }
        public string ParentModuleName { get; set; }
        public int CuratorServiceNumber { get; set; }
        public int SupervisorServiceNumber { get; set; }
    }
}