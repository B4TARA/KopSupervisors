using KOP.Common.Enums;
using KOP.DAL.Entities;
using KOP.DAL.Interfaces;
using KOP.EmailService;
using KOP.Import.Interfaces;
using KOP.Import.Utils;
using Serilog;

namespace KOP.Import.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ILogger _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        private readonly string _templatesPath;

        public NotificationService(IUnitOfWork unitOfWork, IEmailSender emailSender, ILogger logger)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
            _logger = logger;
            _templatesPath = Path.Combine("C:", "PROJECTS", "KopSupervisors", "Files", "templates");
        }

        private string LoadEmailTemplate(string templateName)
        {
            var templatePath = Path.Combine(_templatesPath, templateName);
            if (!File.Exists(templatePath))
            {
                _logger.Error($"Email template not found at path: {templatePath}");
                throw new FileNotFoundException("Email template not found.", templatePath);
            }

            return File.ReadAllText(templatePath);
        }

        public async Task CheckUsersForNotifications()
        {
            var today = TermManager.GetDate();
            var users = await _unitOfWork.Users.GetAllAsync(includeProperties: "Grades");
            var pendingAssessmentResults = await _unitOfWork.AssessmentResults.GetAllAsync(x => x.SystemStatus == SystemStatuses.PENDING);

            foreach (var user in users)
            {
                try
                {
                    // Руководителям 1 и 10 числа месяца оценки
                    // Оценить КК, УК и Назначить группу оценки КК
                    if ((today.Day == 1 || today.Day == 7) && user.SystemRoles.Contains(SystemRoles.Supervisor))
                    {
                        var subordinateUsers = await GetSubordinateUsers(user.Id);
                        var subordinateUsersWithThisMonthPendingGrade = subordinateUsers
                            .Where(x => x.Grades
                                .Any(x =>
                                    x.DateOfCreation.Month == today.Month &&
                                    x.DateOfCreation.Year == today.Year &&
                                    x.SystemStatus == SystemStatuses.PENDING));

                        foreach (var subordinateUser in subordinateUsersWithThisMonthPendingGrade)
                        {
                            // Функция находит самого первого руководителя
                            // Если текущий руководитель не является самым первым (непосредственным), то существует другой
                            var supervisor = GetSupervisorForUser(subordinateUser.Id);

                            if(supervisor.Id != user.Id)
                            {
                                continue;
                            }

                            var template = string.Empty;

                            if (today.Day == 1)
                            {
                                template = LoadEmailTemplate("CreateAssessmentGroupNotificationTemplate.html"); 
                            }
                            else if (today.Day == 7)
                            {
                                template = LoadEmailTemplate("CreateAssessmentGroupReminderTemplate.html");
                            }

                            var mailBody = template.Replace("{SubordinateUserFullName}", subordinateUser.FullName).Replace("{SubordinateUserContractEndDate}", subordinateUser.ContractEndDate.ToString());
                            var mailTitle = $"КОП. Оценка {subordinateUser.FullName}: назначьте группу оценки корпоративных компетенций";
                            var message = new Message([user.Email], mailTitle, mailBody, user.FullName);

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
                            if (today.Day == 1)
                            {
                                var template = LoadEmailTemplate("CreateStrategicTasksAndSelfAssessmentNotificationTemplate.html");
                                var mailBody = template.Replace("{UserContractEndDate}", user.ContractEndDate.ToString());
                                var mailTitle = $"КОП. Самооценка {user.FullName}";

                                var message = new Message([user.Email], mailTitle, mailBody, user.FullName);
                                await _emailSender.SendEmailAsync(message);
                            }
                            else if (today.Day == 15)
                            {
                                var template = LoadEmailTemplate("CreateStrategicTasksAndSelfAssessmentReminderTemplate.html");
                                var mailBody = template.Replace("{UserContractEndDate}", user.ContractEndDate.ToString());
                                var mailTitle = $"КОП. Самооценка {user.FullName}";

                                var message = new Message([user.Email], mailTitle, mailBody, user.FullName);
                                await _emailSender.SendEmailAsync(message);
                            }
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

                            if (today.Day == 1)
                            {
                                var template = LoadEmailTemplate("CreateAssessmentCriteriaNotificationTemplate.html");

                                foreach (var subordinateUser in usersWithThisMonthPendingGrade)
                                {
                                    var mailBody = template.Replace("{SubordinateUserFullName}", subordinateUser.FullName);
                                    var mailTitle = $"КОП. Заполнение критериев оценки {subordinateUser.FullName}";

                                    var message = new Message([user.Email], mailTitle, mailBody, user.FullName);
                                    await _emailSender.SendEmailAsync(message);
                                }
                            }
                            else if (today.Day == 15)
                            {
                                var template = LoadEmailTemplate("CreateAssessmentCriteriaReminderTemplate.html");

                                foreach (var subordinateUser in usersWithThisMonthPendingGrade)
                                {
                                    var mailBody = template.Replace("{SubordinateUserFullName}", subordinateUser.FullName);
                                    var mailTitle = $"КОП. Заполнение критериев оценки {subordinateUser.FullName}";

                                    var message = new Message([user.Email], mailTitle, mailBody, user.FullName);
                                    await _emailSender.SendEmailAsync(message);
                                }
                            }
                        }
                    }
                    // Группе оценки КК + HR (Отдел развития УРП) 10 и 20 числа месяца оценки
                    // Оценить КК
                    if (today.Day == 10 || today.Day == 20)
                    {
                        var appointedThisMonthCorporateAssessmentResults = await _unitOfWork.AssessmentResults
                            .GetAllAsync(x =>
                                x.JudgeId == user.Id && // Пользователь является оценщиком
                                (x.Type == AssessmentResultTypes.ColleagueAssessment || x.Type == AssessmentResultTypes.UrpAssessment) &&
                                x.DateOfCreation.Month == today.Month &&
                                x.DateOfCreation.Year == today.Year &&
                                x.SystemStatus == SystemStatuses.PENDING &&
                                x.Assessment.AssessmentType.SystemAssessmentType == SystemAssessmentTypes.СorporateСompetencies,
                                includeProperties: ["Assessment.User"]);

                        foreach (var assessmentResult in appointedThisMonthCorporateAssessmentResults)
                        {
                            if (today.Day == 10)
                            {
                                var template = LoadEmailTemplate("CreateCorporateCompeteciesAssessmentNotificationTemplate.html");
                                var mailBody = template.Replace("{SubordinateUserFullName}", assessmentResult.Assessment.User.FullName);
                                var mailTitle = $"КОП. Оцените корпоративные компетенции {assessmentResult.Assessment.User.FullName}";

                                var message = new Message([user.Email], mailTitle, mailBody, user.FullName);
                                await _emailSender.SendEmailAsync(message);

                            }
                            else if (today.Day == 20)
                            {
                                var template = LoadEmailTemplate("CreateCorporateCompeteciesAssessmentReminderTemplate.html");
                                var mailBody = template.Replace("{SubordinateUserFullName}", assessmentResult.Assessment.User.FullName);
                                var mailTitle = $"КОП. Оцените корпоративные компетенции {assessmentResult.Assessment.User.FullName}";

                                var message = new Message([user.Email], mailTitle, mailBody, user.FullName);
                                await _emailSender.SendEmailAsync(message);

                            }
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
                            var template = LoadEmailTemplate("CreateCorporateCompeteciesAssessmentReminderTemplate.html");

                            foreach (var userWithPrevMonthGrade in usersWithPreviousMonthPendingGrade)
                            {
                                var mailBody = template;
                                var mailTitle = $"КОП. {userWithPrevMonthGrade.FullName}. Заполните краткие выводы";

                                var message = new Message([user.Email], mailTitle, mailBody, user.FullName);
                                await _emailSender.SendEmailAsync(message);
                            }
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
                            if (today.Day == 5)
                            {
                                var template = LoadEmailTemplate("CreateValueJudgmentAndApproveEmployeeNotificationTemplate.html");

                                foreach (var subUserWithPrevMonthGrade in subordinateUsersWithPreviousMonthPendingGrade)
                                {
                                    var mailBody = template.Replace("{UserFullName}", subUserWithPrevMonthGrade.FullName).Replace("{SubordinateUserContractEndDate}", subUserWithPrevMonthGrade.ContractEndDate.ToString());
                                    var mailTitle = $"КОП. {subUserWithPrevMonthGrade.FullName}. Ознакомьтесь с результатами оценки";

                                    var message = new Message([user.Email], mailTitle, mailBody, user.FullName);
                                    await _emailSender.SendEmailAsync(message);
                                }
                            }
                            else if (today.Day == 8)
                            {
                                var template = LoadEmailTemplate("CreateValueJudgmentAndApproveEmployeeReminderTemplate.html");

                                foreach (var subUserWithPrevMonthGrade in subordinateUsersWithPreviousMonthPendingGrade)
                                {
                                    var mailBody = template.Replace("{UserFullName}", subUserWithPrevMonthGrade.FullName).Replace("{SubordinateUserContractEndDate}", subUserWithPrevMonthGrade.ContractEndDate.ToString());
                                    var mailTitle = $"КОП. {subUserWithPrevMonthGrade.FullName}. Ознакомьтесь с результатами оценки";

                                    var message = new Message([user.Email], mailTitle, mailBody, user.FullName);
                                    await _emailSender.SendEmailAsync(message);
                                }
                            }
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
                            if (today.Day == 10)
                            {
                                var template = LoadEmailTemplate("CreateApprovementByEmployeeNotificationTemplate.html");
                                var mailBody = template.Replace("{SubordinateUserContractEndDate}", user.ContractEndDate.ToString());
                                var mailTitle = $"КОП. {user.FullName}. Ознакомьтесь с результатами оценки";

                                var message = new Message([user.Email], mailTitle, mailBody, user.FullName);
                                await _emailSender.SendEmailAsync(message);

                            }
                            else if (today.Day == 13)
                            {
                                var template = LoadEmailTemplate("CreateApprovementByEmployeeReminderTemplate.html");
                                var mailBody = template.Replace("{SubordinateUserContractEndDate}", user.ContractEndDate.ToString());
                                var mailTitle = $"КОП. {user.FullName}. Ознакомьтесь с результатами оценки";

                                var message = new Message([user.Email], mailTitle, mailBody, user.FullName);
                                await _emailSender.SendEmailAsync(message);
                            }
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
                            var template = LoadEmailTemplate("CreateReportAndCheckAssessmentCompletionNotificationTemplate.html");

                            foreach (var userWithPrevMonthGrade in usersWithPreviousMonthPendingGrade)
                            {
                                var mailBody = template.Replace("{SubordinateUserFullName}", userWithPrevMonthGrade.FullName);
                                var mailTitle = $"КОП. {userWithPrevMonthGrade.FullName}. Выгрузите результаты оценки";

                                var message = new Message([user.Email], mailTitle, mailBody, user.FullName);
                                await _emailSender.SendEmailAsync(message);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error($"Error while checking user with ID {user.Id} for notifications: {ex.Message}");
                    continue;
                }
            }
        }

        private async Task<List<User>> GetSubordinateUsers(int supervisorId)
        {
            var supervisor = await _unitOfWork.Users.GetAsync(x => x.Id == supervisorId, includeProperties: [
                    "SubordinateSubdivisions.Users.Grades",
                    "SubordinateSubdivisions.Children.Users.Grades"
                    ]);

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

        private async Task<List<User>> GetSubordinateUsers(Subdivision subdivision)
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

        private async Task<User?> GetSupervisorForUser(int userId)
        {
            var user = await _unitOfWork.Users.GetAsync(x => x.Id == userId);
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
    }
}