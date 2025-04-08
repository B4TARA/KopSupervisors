using KOP.Common.Enums;
using KOP.DAL.Entities;
using KOP.DAL.Interfaces;
using KOP.EmailService;
using KOP.Import.Interfaces;
using KOP.Import.Utils;

namespace KOP.Import.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        public NotificationService(IUnitOfWork unitOfWork, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
        }

        public async Task CheckUsersForNotifications()
        {
            var today = TermManager.GetDate();
            var users = await _unitOfWork.Users.GetAllAsync(includeProperties: "Grades");
            var mails = await _unitOfWork.Mails.GetAllAsync();
            var pendingAssessmentResults = await _unitOfWork.AssessmentResults.GetAllAsync(x => x.SystemStatus == SystemStatuses.PENDING);

            foreach (var user in users)
            {
                try
                {
                    // Руководителям 1 и 10 числа месяца оценки
                    // Оценить КК, УК и Назначить группу оценки КК
                    if ((today.Day == 1 || today.Day == 10) && user.SystemRoles.Contains(SystemRoles.Supervisor))
                    {
                        if (user.SystemRoles.Contains(SystemRoles.Curator) && user.Id != 119)
                        {
                            // Не надо уведомление
                        }
                        else
                        {
                            var subordinateUsers = await GetSubordinateUsers(user.Id);
                            var subordinateUsersWithThisMonthPendingGrade = subordinateUsers
                                .Where(x => x.Grades
                                    .Any(x =>
                                        x.DateOfCreation.Month == today.Month &&
                                        x.DateOfCreation.Year == today.Year &&
                                        x.SystemStatus == SystemStatuses.PENDING));

                            //// Есть подчиненные с назначенными оценками в текущем месяце
                            //if (subordinateUsersWithThisMonthPendingGrade.Any())
                            //{
                            //    Mail? mail = null;

                            //    if (today.Day == 1)
                            //    {
                            //        mail = mails.FirstOrDefault(x => x.Code == MailCodes.CreateAssessmentGroupAndAssessEmployeeNotification);
                            //    }
                            //    else if (today.Day == 10)
                            //    {
                            //        mail = mails.FirstOrDefault(x => x.Code == MailCodes.CreateAssessmentGroupAndAssessEmployeeReminder);
                            //    }
                            //}

                            foreach (var subordinateUser in subordinateUsersWithThisMonthPendingGrade)
                            {
                                var mail = new Mail();
                                mail.Body = $"Сообщаем, что контракт с {subordinateUser.FullName} завершается {subordinateUser.ContractEndDate}.\r\nВыберите 3 и более коллег, с которыми взаимодействует оцениваемый работник, для оценки корпоративных компетенций.\r\nВы можете сделать это в системе КОП самостоятельно или обратиться к вашему HR-менеджеру.\r\n\r\n<a href=\"https://kop.mtb.minsk.by/supervisors\" target=\"_blank\">https://kop.mtb.minsk.by/supervisors</a>";
                                mail.Title = mail.Title = $"КОП.Оценка {subordinateUser.FullName}: назначьте группу оценки корпоративных компетенций";

                                var message = new Message([user.Email, "nsakirina@mtb.minsk.by"], mail.Title, mail.Body, user.FullName);
                                await _emailSender.SendEmailAsync(message);
                            }
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
                            //Mail? mail = null;

                            //if (today.Day == 1)
                            //{
                            //    mail = mails.FirstOrDefault(x => x.Code == MailCodes.CreateStrategicTasksAndSelfAssessmentNotification);
                            //}
                            //else if (today.Day == 15)
                            //{
                            //    mail = mails.FirstOrDefault(x => x.Code == MailCodes.CreateStrategicTasksAndSelfAssessmentReminder);
                            //}

                            var mail = new Mail();
                            mail.Body = $"Информируем вас, что срок заключенного с вами контракта истекает {user.ContractEndDate}.\r\n\r\nС целью проведения оценки результатов деятельности и достижений работника при продлении контракта просим вас до 20 числа:\r\n- провести самооценку корпоративных компетенций\r\n- провести самооценку управленческих компетенций\r\n- заполнить результаты деятельности, достигнутые вами при исполнении должностных обязанностей в течение {thisMonthPendingGrade.StartDate} - {thisMonthPendingGrade.EndDate}\r\n\r\n<a href=\"https://kop.mtb.minsk.by/supervisors\" target=\"_blank\">https://kop.mtb.minsk.by/supervisors</a>\r\n";
                            mail.Title = mail.Title = $"КОП. Самооценка {user.FullName}";

                            var message = new Message([user.Email, "nsakirina@mtb.minsk.by"], mail.Title, mail.Body, user.FullName);
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

                            //// Есть сотрудники с назначенными оценками в текущем месяце
                            //if (usersWithThisMonthPendingGrade.Any())
                            //{
                            //    Mail? mail = null;

                            //    if (today.Day == 1)
                            //    {
                            //        mail = mails.FirstOrDefault(x => x.Code == MailCodes.CreateAssessmentCriteriaNotification);
                            //    }
                            //    else if (today.Day == 10)
                            //    {
                            //        mail = mails.FirstOrDefault(x => x.Code == MailCodes.CreateAssessmentCriteriaReminder);
                            //    }

                            //    if (mail == null)
                            //    {
                            //        await HandleErrorAsync("Не найдено сообщение для отправки ответственным за заполнение критериев 1 или 15 числа месяца оценки");
                            //    }
                            //    else
                            //    {
                            //        var message = new Message([user.Email], mail.Title, mail.Body, user.FullName);
                            //        await _emailSender.SendEmailAsync(message);
                            //    }
                            //}

                            foreach (var subordinateUser in usersWithThisMonthPendingGrade)
                            {
                                var mail = new Mail();
                                mail.Body = $"Для проведения оценки руководителя {subordinateUser.FullName} просим до 20 числа заполнить закрепленные за вами критерии оценки. \r\n\r\n<a href=\"https://kop.mtb.minsk.by/supervisors\" target=\"_blank\">https://kop.mtb.minsk.by/supervisors</a>";
                                mail.Title = mail.Title = $"КОП. Заполнение критериев оценки  {subordinateUser.FullName}";

                                var message = new Message([user.Email, "nsakirina@mtb.minsk.by"], mail.Title, mail.Body, user.FullName);
                                await _emailSender.SendEmailAsync(message);
                            }
                        }
                    }
                    //// Группе оценки КК + HR (Отдел развития УРП) 15 числа месяца оценки
                    //// Оценить КК
                    //if (today.Day == 15)
                    //{
                    //    var appointedThisMonthCorporateAssessmentResults = await _unitOfWork.AssessmentResults
                    //        .GetAllAsync(x =>
                    //            x.JudgeId == user.Id && // Пользователь является оценщиком
                    //            x.Assessment.UserId != user.Id && // Не самооценка
                    //            x.DateOfCreation.Month == today.Month &&
                    //            x.DateOfCreation.Year == today.Year &&
                    //            x.SystemStatus == SystemStatuses.PENDING &&
                    //            x.Assessment.AssessmentType.SystemAssessmentType == SystemAssessmentTypes.СorporateСompetencies, includeProperties: "Assessment");

                    //    foreach (var assessmentResult in appointedThisMonthCorporateAssessmentResults)
                    //    {
                    //        var userId = assessmentResult.Assessment.UserId;
                    //        var supervisorForUser = GetSupervisorForUser(userId);

                    //        // Если пользователь является непосредственным руководителем оцениваемого
                    //        if (supervisorForUser != null && supervisorForUser.Id == user.Id)
                    //        {
                    //            continue;
                    //        }

                    //        var mail = mails.FirstOrDefault(x => x.Code == MailCodes.CreateCorporateCompeteciesAssessmentNotification);

                    //        if (mail == null)
                    //        {
                    //            await HandleErrorAsync("Не найдено сообщение для отправки оценщикам 15 числа месяца оценки");
                    //        }
                    //        else
                    //        {
                    //            var message = new Message([user.Email], mail.Title, mail.Body, user.FullName);
                    //            await _emailSender.SendEmailAsync(message);
                    //        }
                    //    }
                    //}
                    //// УРП 1 числа месяца, следующего за месяцем оценки
                    //// Оставить выводы по криетриям: результаты деятельности, КПЭ, квалификация
                    //if (today.Day == 1 && user.SystemRoles.Contains(SystemRoles.Urp))
                    //{
                    //    var usersWithPreviousMonthPendingGrade = users
                    //        .Where(x => x.Grades
                    //            .Any(x =>
                    //                x.DateOfCreation.AddMonths(1).Month == today.Month &&
                    //                x.DateOfCreation.Year == today.Year &&
                    //                x.SystemStatus == SystemStatuses.PENDING));

                    //    // Есть сотрудники с назначенными оценками в прошлом месяце
                    //    if (usersWithPreviousMonthPendingGrade.Any())
                    //    {
                    //        var mail = mails.FirstOrDefault(x => x.Code == MailCodes.CreateConclusionsAndCheckGradeNotification);

                    //        if (mail == null)
                    //        {
                    //            await HandleErrorAsync("Не найдено сообщение для отправки УРП 1 числа месяца, следующего за месяцем оценки");
                    //        }
                    //        else
                    //        {
                    //            var message = new Message([user.Email], mail.Title, mail.Body, user.FullName);
                    //            await _emailSender.SendEmailAsync(message);
                    //        }
                    //    }
                    //}
                    //// Непосредственным руководителям 5 и 8 числа месяца, следующего за месяцем оценки
                    //// Оставить ОС от руководителя и ознакомиться с результатами оценки
                    //if ((today.Day == 5 || today.Day == 8) && user.SystemRoles.Contains(SystemRoles.Supervisor))
                    //{
                    //    var subordinateUsers = await GetSubordinateUsers(user.Id);
                    //    var subordinateUsersWithPreviousMonthPendingGrade = subordinateUsers
                    //        .Where(x => x.Grades
                    //            .Any(x =>
                    //                x.DateOfCreation.AddMonths(1).Month == today.Month &&
                    //                x.DateOfCreation.Year == today.Year &&
                    //                x.SystemStatus == SystemStatuses.PENDING));

                    //    // Есть сотрудники с назначенными оценками в прошлом месяце
                    //    if (subordinateUsersWithPreviousMonthPendingGrade.Any())
                    //    {
                    //        Mail? mail = null;

                    //        if (today.Day == 5)
                    //        {
                    //            mail = mails.FirstOrDefault(x => x.Code == MailCodes.CreateValueJudgmentAndApproveEmployeeNotification);
                    //        }
                    //        else if (today.Day == 8)
                    //        {
                    //            mail = mails.FirstOrDefault(x => x.Code == MailCodes.CreateValueJudgmentAndApproveEmployeeReminder);
                    //        }

                    //        if (mail == null)
                    //        {
                    //            await HandleErrorAsync("Не найдено сообщение для отправки непосредственным руковолителям 5 или 8 числа месяца, следующего за месяцем оценки");
                    //        }
                    //        else
                    //        {
                    //            var message = new Message([user.Email], mail.Title, mail.Body, user.FullName);
                    //            await _emailSender.SendEmailAsync(message);
                    //        }
                    //    }
                    //}
                    //// Оцениваемым сотрудникам 10 и 13 числа месяца, следующего за месяцем оценки
                    //// Ознакомиться с результатами оценки
                    //if ((today.Day == 10 || today.Day == 13) && user.SystemRoles.Contains(SystemRoles.Employee))
                    //{
                    //    var thisMonthPendingGrade = user.Grades
                    //        .FirstOrDefault(x =>
                    //            x.DateOfCreation.AddMonths(1).Month == today.Month &&
                    //            x.DateOfCreation.Year == today.Year &&
                    //            x.SystemStatus == SystemStatuses.PENDING);

                    //    // Есть назначенные оценки в прошлом месяце
                    //    if (thisMonthPendingGrade != null)
                    //    {
                    //        Mail? mail = null;

                    //        if (today.Day == 10)
                    //        {
                    //            mail = mails.FirstOrDefault(x => x.Code == MailCodes.CreateApprovementByEmployeeNotification);
                    //        }
                    //        else if (today.Day == 13)
                    //        {
                    //            mail = mails.FirstOrDefault(x => x.Code == MailCodes.CreateApprovementByEmployeeReminder);
                    //        }

                    //        if (mail == null)
                    //        {
                    //            await HandleErrorAsync("Не найдено сообщение для отправки оцениваемым сотрудникам 10 или 13 числа месяца, следующего за месяцем оценки");
                    //        }
                    //        else
                    //        {
                    //            var message = new Message([user.Email], mail.Title, mail.Body, user.FullName);
                    //            await _emailSender.SendEmailAsync(message);
                    //        }
                    //    }
                    //}
                    //// УРП 15 числа месяца, следующего за месяцем оценки
                    //// Выгрузить результаты оценки
                    //if (today.Day == 15 && user.SystemRoles.Contains(SystemRoles.Urp))
                    //{
                    //    var usersWithPreviousMonthPendingGrade = users
                    //        .Where(x => x.Grades
                    //            .Any(x =>
                    //                x.DateOfCreation.AddMonths(1).Month == today.Month &&
                    //                x.DateOfCreation.Year == today.Year &&
                    //                x.SystemStatus == SystemStatuses.PENDING));

                    //    // Есть сотрудники с назначенными оценками в прошлом месяце
                    //    if (usersWithPreviousMonthPendingGrade.Any())
                    //    {
                    //        var mail = mails.FirstOrDefault(x => x.Code == MailCodes.CreateReportAndCheckAssessmentCompletionNotification);

                    //        if (mail == null)
                    //        {
                    //            await HandleErrorAsync("Не найдено сообщение для отправки УРП 15 числа месяца, следующего за месяцем оценки");
                    //        }
                    //        else
                    //        {
                    //            var message = new Message([user.Email], mail.Title, mail.Body, user.FullName);
                    //            await _emailSender.SendEmailAsync(message);
                    //        }
                    //    }
                    //}
                }
                catch (Exception ex)
                {
                    await HandleErrorAsync(ex.Message);
                }
            }
        }

        private async Task<IEnumerable<User>> GetSubordinateUsers(int supervisorId)
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

        private async Task<IEnumerable<User>> GetSubordinateUsers(Subdivision subdivision)
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