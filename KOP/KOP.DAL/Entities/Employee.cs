using KOP.Common.Enums;
using KOP.DAL.Entities.AssessmentEntities;
using KOP.DAL.Entities.GradeEntities;

namespace KOP.DAL.Entities
{
    public class Employee
    {
        public int Id { get; set; }
        public int ServiceNumber { get; set; } // Табельный номер сотрудника
        public string? FullName { get; set; } // ФИО сотрудника
        public string? Position { get; set; } // Должность сотрудника
        public string? Subdivision { get; set; } // Подразделение сотрудника
        public string? GradeGroup { get; set; } // Группа грейда сотрудника
        public DateOnly? HireDate { get; set; } // Дата приема на работу
        public DateOnly? ContractStartDate { get; set; } // Дата начала контракта
        public DateOnly? ContractEndDate { get; set; } // Дата окончания контракта

        public string? Login { get; set; } // Логин сотрудника для входа в систему
        public string? Password { get; set; } // Пароль сотрудника для входа в систему
        public string? Email { get; set; } // Рабочая почта сотрудника (для рассылки уведомлений и восстановления пароля)
        public string ImagePath { get; set; } // Путь к аватарке сотрудника
        public bool IsSuspended { get; set; } // Флаг ограничения доступа сотрудника ко входу в систему

        public List<SystemRoles> SystemRoles { get; set; } // Роли сотрудника  
        public List<Grade> Grades { get; set; } // Оценки карьерного роста сотрудника
        public List<Assessment> Assessments { get; set; } // качественные оценки сотрудника
        public List<Module> Modules { get; set; } // Модули, к которым относится данный сотрудник

        public DateOnly DateOfCreation { get; set; }

        public Employee()
        {
            ImagePath = "/default_profile_icon.svg";
            IsSuspended = false;
            SystemRoles = new();
            Grades = new();
            Assessments = new();
            Modules = new();
            DateOfCreation = DateOnly.FromDateTime(DateTime.Today);
        }

        public string GetWorkPeriod
        {
            get
            {
                var monthDay = new int[12] { 31, -1, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
                var currentDate = DateTime.Now;
                var increment = 0;
                var day = 0;
                var month = 0;
                var year = 0;

                if (HireDate is null)
                {
                    return "Данные о приеме на работу отсутствуют";
                }
                else if (HireDate.Value.Day > currentDate.Day)
                {
                    increment = monthDay[HireDate.Value.Month - 1];
                }

                if (increment == -1)
                {
                    if (DateTime.IsLeapYear(HireDate.Value.Year))
                    {
                        increment = 29;
                    }
                    else
                    {
                        increment = 28;
                    }
                }

                if (increment != 0)
                {
                    day = (currentDate.Day + increment) - HireDate.Value.Day;
                    increment = 1;
                }
                else
                {
                    day = currentDate.Day - HireDate.Value.Day;
                }

                if ((HireDate.Value.Month + increment) > currentDate.Month)
                {
                    month = (currentDate.Month + 12) - (HireDate.Value.Month + increment);
                    increment = 1;
                }
                else
                {
                    month = (currentDate.Month) - (HireDate.Value.Month + increment);
                    increment = 0;
                }

                year = currentDate.Year - (HireDate.Value.Year + increment);

                var yearsString = (year > 4 || year == 0) ? "л." : "г.";
                var durationString = $"{year} {yearsString} {month} мес. {day} дн.";

                return durationString;
            }
        }
    }
}