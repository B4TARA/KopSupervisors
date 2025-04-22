using ExcelDataReader;
using KOP.Common.Enums;
using KOP.DAL.Entities;
using KOP.DAL.Entities.AssessmentEntities;
using KOP.DAL.Entities.GradeEntities;
using KOP.DAL.Interfaces;
using KOP.EmailService;
using KOP.Import.Interfaces;
using KOP.Import.Utils;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Serilog;
using System.Data;
using System.Xml;
using SystemRoles = KOP.Common.Enums.SystemRoles;

namespace KOP.Import.Services
{
    public class ExportAndImport : IExportAndImport
    {
        private readonly ILogger _logger;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _config;
        private readonly IUnitOfWork _unitOfWork;

        private readonly string _additionalUsersServiceNumbersPath;
        private readonly string _usersInfosPath;
        private readonly string _usersStructuresPath;
        private readonly string _colsArrayPath;
        private readonly string _usersPassContainerPath;
        private readonly string _usersImgDownloadPath;
        private readonly string _trainingEventsPath;

        private List<int> additionalUsersServiceNumbers;

        public ExportAndImport(ILogger logger, IEmailSender emailSender, IConfiguration config, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _emailSender = emailSender;
            _config = config;
            _unitOfWork = unitOfWork;
            additionalUsersServiceNumbers = new();

            _additionalUsersServiceNumbersPath = _config["FilePaths:AdditionalUsersServiceNumbersPath"] ?? "";
            _usersInfosPath = _config["FilePaths:UsersInfosPath"] ?? "";
            _usersStructuresPath = _config["FilePaths:UsersStructuresPath"] ?? "";
            _colsArrayPath = _config["FilePaths:ColsArrayPath"] ?? "";
            _usersPassContainerPath = _config["FilePaths:UsersPassContainerPath"] ?? "";
            _usersImgDownloadPath = _config["FilePaths:UsersImgDownloadPath"] ?? "";
            _trainingEventsPath = _config["FilePaths:TrainingEventsPath"] ?? "";
        }

        private async Task ValidateFilePaths()
        {
            if (string.IsNullOrEmpty(_additionalUsersServiceNumbersPath))
            {
                await HandleErrorAsync("_additionalUsersServiceNumbersPath не задан.");
                return;
            }
            if (string.IsNullOrEmpty(_usersInfosPath))
            {
                await HandleErrorAsync("_usersInfosPath не задан.");
                return;
            }
            if (string.IsNullOrEmpty(_usersStructuresPath))
            {
                await HandleErrorAsync("_usersStructuresPath не задан.");
                return;
            }
            if (string.IsNullOrEmpty(_colsArrayPath))
            {
                await HandleErrorAsync("_colsArrayPath не задан.");
                return;
            }
            if (string.IsNullOrEmpty(_usersPassContainerPath))
            {
                await HandleErrorAsync("_usersPassContainerPath не задан.");
                return;
            }
            if (string.IsNullOrEmpty(_usersImgDownloadPath))
            {
                await HandleErrorAsync("_usersImgDownloadPath не задан.");
                return;
            }
            if (string.IsNullOrEmpty(_trainingEventsPath))
            {
                await HandleErrorAsync("_trainingEventsPath не задан.");
                return;
            }
        }

        public async Task TransferDataFromExcelToDatabase()
        {
            try
            {
                Log.Information("Обработка файлов с пользователями:");
                await ValidateFilePaths();
                var usersFromExcel = await ProcessUsers();

                Log.Information("Обработка пользователей:");
                Log.Information("Синхронизация с БД...");
                await PutUsersInDatabase(usersFromExcel);

                Log.Information("Блокировка неактивных пользователей...");
                await BlockNonActiveUsers(usersFromExcel);

                Log.Information("Обработка файла с мероприятиями:");
                await ProcessTrainingEvents(_trainingEventsPath);
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex.Message);
            }
        }

        private async Task<List<User>> ProcessUsers()
        {
            Log.Information("Чтение дополнительных табельных номеров ...");
            additionalUsersServiceNumbers = ReadAdditionalUsersServiceNumbersFromFile(_additionalUsersServiceNumbersPath);

            Log.Information("Чтение данных из ШР ...");
            var usersFromExcel = await ReadUsersStructuresFromFile(_usersStructuresPath);
            var usersFromExcelList = usersFromExcel.ToList();

            Log.Information("Чтение данных из СЧ ...");
            return await ReadUsersInfosFromFile(_usersInfosPath, usersFromExcelList);
        }

        private async Task PopulateUserWithModule(User userFromExcel)
        {
            if (IsAdditionalSubdivision(userFromExcel.SubdivisionFromFile, userFromExcel.DepartmentFromFile) && !IsMeetUserStructureBusinessRequirements(userFromExcel.StructureRole, userFromExcel.GradeGroup))
            {
                return;
            }

            var subdivision = await _unitOfWork.Subdivisions.GetAsync(x => x.Name.Replace(" ", "").ToLower() == userFromExcel.SubdivisionFromFile.Replace(" ", "").ToLower());

            if (subdivision == null)
            {
                return;
            }

            userFromExcel.ParentSubdivision = subdivision;
        }

        private void PopulateUserWithXmlData(User userFromExcel, XmlDocument colsArrayDocument, XmlDocument usersPassContainerDocument)
        {
            if (userFromExcel.FullName == null)
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
                var base64Image = Convert.ToString(obj["pict_url"]);
                var fileName = userFromExcel.FullName.Replace(" ", "");
                var imagePath = ImageUtilities.SaveBase64Image(base64Image, fileName, _usersImgDownloadPath);

                userFromExcel.ImagePath = imagePath;
                userFromExcel.Email = email;
            }

            var userNode2 = usersPassContainerDocument.SelectSingleNode(xpath);

            if (userNode1 != null && userNode2 != null)
            {
                var obj = JObject.Parse(userNode2.InnerText);

                var login = Convert.ToString(obj["login"]) ?? "-";
                var password = Convert.ToString(obj["password"]) ?? "-";

                userFromExcel.Login = login;
                userFromExcel.Password = password;
            }
        }

        private async Task PopulateUserWithRole(User userFromExcel)
        {
            if (userFromExcel.SubdivisionFromFile == null)
            {
                return;
            }
            else if (userFromExcel.SubdivisionFromFile.Contains("АДМ Администрация"))
            {
                userFromExcel.SystemRoles.Add(SystemRoles.Curator);
                userFromExcel.SystemRoles.Add(SystemRoles.Supervisor);
            }
            else if (userFromExcel.SubdivisionFromFile.Contains("УМСТ "))
            {
                userFromExcel.SystemRoles.Add(SystemRoles.Umst);

                var rootSubdivisions = await _unitOfWork.Subdivisions.GetAllAsync(x => x.NestingLevel == 1);

                userFromExcel.SubordinateSubdivisions = rootSubdivisions.ToList();
            }
            else if (userFromExcel.SubdivisionFromFile.Contains("ЦУП "))
            {
                userFromExcel.SystemRoles.Add(SystemRoles.Cup);

                var rootSubdivisions = await _unitOfWork.Subdivisions.GetAllAsync(x => x.NestingLevel == 1);

                userFromExcel.SubordinateSubdivisions = rootSubdivisions.ToList();
            }
            else if (userFromExcel.SubdivisionFromFile.Contains("УРП "))
            {
                userFromExcel.SystemRoles.Add(SystemRoles.Urp);

                var rootSubdivisions = await _unitOfWork.Subdivisions.GetAllAsync(x => x.NestingLevel == 1);

                userFromExcel.SubordinateSubdivisions = rootSubdivisions.ToList();
            }
            else if (userFromExcel.SubdivisionFromFile.Contains("УОП "))
            {
                userFromExcel.SystemRoles.Add(SystemRoles.Uop);

                var rootSubdivisions = await _unitOfWork.Subdivisions.GetAllAsync(x => x.NestingLevel == 1);

                userFromExcel.SubordinateSubdivisions = rootSubdivisions.ToList();
            }
        }

        private async Task PutUsersInDatabase(List<User> usersFromExcel)
        {
            foreach (var userFromExcel in usersFromExcel)
            {
                try
                {
                    await PopulateUserWithRole(userFromExcel);
                    await PopulateUserWithModule(userFromExcel);

                    var existingUser = await _unitOfWork.Users.GetAsync(x => x.ServiceNumber == userFromExcel.ServiceNumber, includeProperties: "ParentSubdivision");

                    if (existingUser == null)
                    {
                        await _unitOfWork.Users.AddAsync(userFromExcel);
                        await _unitOfWork.CommitAsync();
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
                    existingUser.SubdivisionFromFile = userFromExcel.SubdivisionFromFile;
                    existingUser.DepartmentFromFile = userFromExcel.DepartmentFromFile;
                    //existingUser.SystemRoles = userFromExcel.SystemRoles;

                    _unitOfWork.Users.Update(existingUser);
                    await _unitOfWork.CommitAsync();
                }
                catch (Exception ex)
                {
                    await HandleErrorAsync($"[ExportAndImport.PutUsersInDatabase]: exception while process user with ServiceNumber {userFromExcel.ServiceNumber}: {ex.Message}");
                    continue;
                }
            }
        }      

        public async Task CheckUsersForGradeProcess()
        {

            var employees = await _unitOfWork.Users.GetAllAsync(x => x.SystemRoles.Contains(SystemRoles.Employee) && !x.IsSuspended, includeProperties: "Grades");

            foreach (var employee in employees)
            {
                try
                {
                    var lastGrade = employee.Grades.OrderByDescending(x => x.Number).FirstOrDefault();

                    if (lastGrade != null && ReadyForEmployeeApproval(employee, lastGrade))
                    {
                        lastGrade.GradeStatus = GradeStatuses.READY_FOR_EMPLOYEE_APPROVAL;
                        _unitOfWork.Grades.Update(lastGrade);
                        await _unitOfWork.CommitAsync();

                        continue;
                    }
                    else if (lastGrade != null && ReadyForSupervisorEmployeeApproval(employee, lastGrade))
                    {
                        lastGrade.GradeStatus = GradeStatuses.READY_FOR_SUPERVISOR_APPROVAL;
                        _unitOfWork.Grades.Update(lastGrade);
                        await _unitOfWork.CommitAsync();

                        continue;
                    }
                    else if (lastGrade != null && lastGrade.SystemStatus != SystemStatuses.PENDING)
                    {
                        continue;
                    }

                    var currentDay = TermManager.GetDate().Day;
                    var currentMonth = TermManager.GetDate().Month;
                    var currentYear = TermManager.GetDate().Year;
                    var gradeStartMonth = employee.ContractEndDate.AddMonths(-4).Month;
                    var gradeStartYear = employee.ContractEndDate.AddMonths(-4).Year;

                    if (currentDay != 1 || currentMonth != gradeStartMonth || currentYear != gradeStartYear)
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
                        ValueJudgment = new ValueJudgment
                        {
                            Strengths = "",
                            BehaviorToCorrect = "",
                            RecommendationsForDevelopment = "",
                        },
                        QualificationConclusion = "Руководитель соответствует квалификационным требованиям и требованиям к деловой репутации.",
                    };

                    var firstDayOfCurrentMonth = new DateOnly(TermManager.GetDate().Year, TermManager.GetDate().Month, 1);
                    var lastDayOfPreviousMonth = firstDayOfCurrentMonth.AddDays(-1);
                    newGrade.EndDate = lastDayOfPreviousMonth;

                    await _unitOfWork.Grades.AddAsync(newGrade);

                    var assessmentTypes = await _unitOfWork.AssessmentTypes.GetAllAsync();
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

                        await _unitOfWork.Assessments.AddAsync(newAssessment);
                    }

                    await _unitOfWork.CommitAsync();
                }
                catch (Exception ex)
                {
                    await _unitOfWork.RollbackAsync();
                    await HandleErrorAsync(ex.Message);
                    continue;
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
            var parentSubdivision = await _unitOfWork.Subdivisions.GetAsync(x => x.Id == user.ParentSubdivisionId, includeProperties: "Parent");

            if (parentSubdivision == null)
            {
                return null;
            }

            var supervisor = await _unitOfWork.Users.GetAsync(x => x.SystemRoles.Contains(SystemRoles.Supervisor) && x.SubordinateSubdivisions.Contains(parentSubdivision));

            if (supervisor != null)
            {
                return supervisor;
            }

            var rootSubdivision = parentSubdivision.Parent;

            while (rootSubdivision != null)
            {
                supervisor = await _unitOfWork.Users.GetAsync(x => x.SystemRoles.Contains(SystemRoles.Supervisor) && x.SubordinateSubdivisions.Contains(rootSubdivision));

                if (supervisor != null)
                {
                    return supervisor;
                }

                rootSubdivision = rootSubdivision.Parent;
            }

            return null;
        }

        private async Task<IEnumerable<User>> GetAllUrpUsers()
        {
            return await _unitOfWork.Users.GetAllAsync(x => x.SystemRoles.Contains(SystemRoles.Urp));
        }

        private async Task BlockNonActiveUsers(IEnumerable<User> usersFromExcel)
        {
            var allDbUsers = await _unitOfWork.Users.GetAllAsync();

            foreach (var dbUser in allDbUsers)
            { 
                // Технологическая учетная запись URP
                if(dbUser.Id == 201)
                {
                    continue;
                }

                try
                {
                    if (!usersFromExcel.Any(x => x.ServiceNumber == dbUser.ServiceNumber))
                    {
                        dbUser.Email = string.Empty;
                        dbUser.IsSuspended = true;
                        dbUser.ParentSubdivisionId = null;

                        _unitOfWork.Users.Update(dbUser);
                        await _unitOfWork.CommitAsync();
                    }
                }
                catch (Exception ex)
                {
                    await HandleErrorAsync(ex.Message);
                    continue;
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

        private bool IsAdditionalSubdivision(string subdivision, string department)
        {
            return subdivision.Contains("ЦУП ") || subdivision.Contains("УМСТ ") || (subdivision.Contains("УРП ") && department.Contains("Отдел развития персонала"));
        }

        private List<int> ReadAdditionalUsersServiceNumbersFromFile(string path)
        {
            var numbers = new List<int>();

            try
            {
                var lines = File.ReadAllLines(path);

                foreach (string line in lines)
                {
                    // Пропускаем пустые строки
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

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
            catch (IOException ioEx)
            {
                _logger.Error($"Ошибка при чтении файла: {ioEx.Message}");
            }
            catch (Exception ex)
            {
                _logger.Error($"Неизвестная ошибка: {ex.Message}");
            }

            return numbers;
        }

        public async Task<List<User>> ReadUsersStructuresFromFile(string filePath)
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
                            GradeGroup = Convert.ToString(table.Rows[rowCounter][44]) ?? "-",
                            StructureRole = Convert.ToString(table.Rows[rowCounter][46]) ?? "-",
                        };

                        if (meetsBusinessRequirements)
                        {
                            userFromExcel.SystemRoles.Add(SystemRoles.Employee);
                        }

                        usersFromExcel.Add(userFromExcel);
                    }
                    catch (InvalidCastException)
                    {
                        continue;
                    }
                    catch (Exception ex)
                    {
                        await HandleErrorAsync(ex.Message);
                        continue;
                    }
                }

                excelDataReader.Close();
            }

            return usersFromExcel;
        }

        public async Task<List<User>> ReadUsersInfosFromFile(string filePath, List<User> usersFromExcel)
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
                        var subdivision = Convert.ToString(table.Rows[rowCounter][22]) ?? "-";
                        var department = Convert.ToString(table.Rows[rowCounter][24]) ?? "-";
                        var userFromExcel = usersFromExcel.FirstOrDefault(x => x.ServiceNumber == serviceNumber);

                        if (IsAdditionalSubdivision(subdivision, department) && userFromExcel == null)
                        {
                            userFromExcel = new User
                            {
                                ServiceNumber = serviceNumber,
                                GradeGroup = "-",
                                StructureRole = "-",
                            };

                            usersFromExcel.Add(userFromExcel);
                        }
                        else if (userFromExcel == null)
                        {
                            continue;
                        }

                        bool meetsBusinessRequirements = IsMeetUserInfoBusinessRequirements(table.Rows[rowCounter], userFromExcel);
                        bool isAdditionalUser = IsAdditionalUser(serviceNumber);

                        if (!meetsBusinessRequirements)
                        {
                            userFromExcel.SystemRoles.Remove(SystemRoles.Employee);

                            if (!isAdditionalUser)
                            {
                                usersFromExcel.Remove(userFromExcel);
                            }
                        }

                        userFromExcel.FullName = Convert.ToString(table.Rows[rowCounter][3]) ?? "-";
                        userFromExcel.Position = Convert.ToString(table.Rows[rowCounter][6]) ?? "-";
                        userFromExcel.HireDate = DateOnly.FromDateTime(Convert.ToDateTime(table.Rows[rowCounter][12]));
                        userFromExcel.SubdivisionFromFile = Convert.ToString(table.Rows[rowCounter][22]) ?? "-";
                        userFromExcel.DepartmentFromFile = Convert.ToString(table.Rows[rowCounter][24]) ?? "-";
                        userFromExcel.ContractStartDate = DateOnly.FromDateTime(Convert.ToDateTime(table.Rows[rowCounter][57]));
                        userFromExcel.ContractEndDate = DateOnly.FromDateTime(Convert.ToDateTime(table.Rows[rowCounter][58]));

                        PopulateUserWithXmlData(userFromExcel, colsArrayDocument, usersPassContainerDocument);
                    }
                    catch (InvalidCastException)
                    {
                        continue;
                    }
                    catch (Exception ex)
                    {
                        await HandleErrorAsync(ex.Message);
                        continue;
                    }
                }

                excelDataReader.Close();
            }

            return usersFromExcel;
        }

        public async Task ProcessTrainingEvents(string filePath)
        {
            var allDbUsers = await _unitOfWork.Users.GetAllAsync(includeProperties: "Grades.TrainingEvents");
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
                        var userFullName = Convert.ToString(table.Rows[rowCounter][0]) ?? "-";
                        var trainingEventFromFileName = Convert.ToString(table.Rows[rowCounter][3]) ?? "-";
                        var trainingEventFromFileStatus = Convert.ToString(table.Rows[rowCounter][6]) ?? "-";
                        var trainingEventFromFileStartDate = DateOnly.FromDateTime(Convert.ToDateTime(table.Rows[rowCounter][4]));
                        var trainingEventFromFileEndDate = DateOnly.FromDateTime(Convert.ToDateTime(table.Rows[rowCounter][5]));
                        var trainingEventFromFileCompetence = Convert.ToString(table.Rows[rowCounter][7]) ?? "-";

                        var dbUser = await _unitOfWork.Users.GetAsync(x => x.FullName.Replace(" ", "").ToLower() == userFullName.Replace(" ", "").ToLower());

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

                        _unitOfWork.Grades.Update(userLastGrade);
                        await _unitOfWork.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        await _unitOfWork.RollbackAsync();
                        await HandleErrorAsync(ex.Message);
                        continue;
                    }
                }

                excelDataReader.Close();
            }
        }     

        private async Task HandleErrorAsync(string errorMessage)
        {
            var addressee = new string[] { "ebaturel@mtb.minsk.by" };
            var messageBody = errorMessage;
            var message = new Message(addressee, "Ошибка импорта", messageBody, "Батурель Евгений Дмитриевич");

            await _emailSender.SendEmailAsync(message);
            _logger.Error(errorMessage);
        }
    }
}