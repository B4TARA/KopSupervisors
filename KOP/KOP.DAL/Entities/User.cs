using KOP.Common.Enums;

namespace KOP.DAL.Entities
{
    public class User
    {
        public int Id { get; set; }
        public int ServiceNumber { get; set; }
        public string FullName { get; set; }
        public string Position { get; set; }
        public string SubdivisionFromFile { get; set; }
        public string DepartmentFromFile { get; set; }
        public string StructureRole { get; set; }
        public string GradeGroup { get; set; }
        public DateOnly? HireDate { get; set; }
        public DateOnly? ContractStartDate { get; set; }
        public DateOnly? ContractEndDate { get; set; }

        public string Login { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string ImagePath { get; set; }
        public bool IsDismissed { get; set; }

        public Subdivision? ParentSubdivision { get; set; }
        public int? ParentSubdivisionId { get; set; }

        public List<SystemRoles> SystemRoles { get; set; } = new();
        public List<Subdivision> SubordinateSubdivisions { get; set; } = new();
        public List<Grade> Grades { get; set; } = new();
        public List<Assessment> Assessments { get; set; } = new();

        public DateTime? LastLogin { get; set; }

        public string GetContractEndDate
        {
            get
            {
                if (!ContractEndDate.HasValue)
                {
                    return "Дата окончания контракта не установлена";
                }        

                var contractEndDate = ContractEndDate.Value.ToString("dd.MM.yyyy");

                return contractEndDate;
            }
        }

        public string GetWorkPeriod
        {
            get
            {
                if (!HireDate.HasValue)
                {
                    return "Дата найма не установлена";
                }

                var monthDay = new int[12] { 31, -1, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
                var currentDate = DateTime.Now;
                var increment = 0;
                var day = 0;
                var month = 0;
                var year = 0;

                // Получаем значение HireDate
                var hireDate = HireDate.Value;

                if (hireDate.Day > currentDate.Day)
                {
                    increment = monthDay[hireDate.Month - 1];
                }

                if (increment == -1)
                {
                    if (DateTime.IsLeapYear(hireDate.Year))
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
                    day = (currentDate.Day + increment) - hireDate.Day;
                    increment = 1;
                }
                else
                {
                    day = currentDate.Day - hireDate.Day;
                }

                if ((hireDate.Month + increment) > currentDate.Month)
                {
                    month = (currentDate.Month + 12) - (hireDate.Month + increment);
                    increment = 1;
                }
                else
                {
                    month = (currentDate.Month) - (hireDate.Month + increment);
                    increment = 0;
                }

                year = currentDate.Year - (hireDate.Year + increment);

                var yearsString = (year > 4 || year == 0) ? "л." : "г.";
                var durationString = $"{year} {yearsString} {month} мес. {day} дн.";

                return durationString;
            }
        }

        public (string Value, bool HasDateValue) GetNextGradeStartDate
        {
            get
            {
                // Проверяем, установлено ли значение ContractEndDate
                if (!ContractEndDate.HasValue)
                {
                    return ("Дата окончания контракта не установлена", false);
                }

                // Проверяем, закончился ли контракт
                if (ContractEndDate < DateOnly.FromDateTime(DateTime.Today))
                {
                    return ("Вероятно, контракт уже закончился", false);
                }

                var tempDate = ContractEndDate.Value.AddMonths(-4);
                var nextGradeStartDate = new DateOnly(tempDate.Year, tempDate.Month, 1);

                // Дата запуска веб-приложения
                // До этой даты оценки не начинались
                if (nextGradeStartDate < new DateOnly(2025, 4, 1))
                {
                    return ("-", false);
                }

                return (nextGradeStartDate.ToString("dd.MM.yyyy"), true);
            }
        }
    }
}