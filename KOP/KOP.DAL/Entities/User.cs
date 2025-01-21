using KOP.Common.Enums;
using KOP.DAL.Entities.AssessmentEntities;
using KOP.DAL.Entities.GradeEntities;

namespace KOP.DAL.Entities
{
    public class User
    {
        public int Id { get; set; }
        public int ServiceNumber { get; set; }
        public string FullName { get; set; }
        public string Position { get; set; }
        public string SubdivisionFromFile { get; set; }
        public string GradeGroup { get; set; }
        public DateOnly HireDate { get; set; }
        public DateOnly ContractStartDate { get; set; }
        public DateOnly ContractEndDate { get; set; }

        public string Login { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string ImagePath { get; set; }
        public bool IsSuspended { get; set; }

        public Subdivision? ParentSubdivision { get; set; }
        public int? ParentSubdivisionId { get; set; }

        public List<Subdivision> SubordinateSubdivisions { get; set; } = new();
        public List<SystemRoles> SystemRoles { get; set; } = new();
        public List<Grade> Grades { get; set; } = new();
        public List<Assessment> Assessments { get; set; } = new();

        public DateOnly DateOfCreation { get; set; } = DateOnly.FromDateTime(DateTime.Today);

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

                if (HireDate.Day > currentDate.Day)
                {
                    increment = monthDay[HireDate.Month - 1];
                }

                if (increment == -1)
                {
                    if (DateTime.IsLeapYear(HireDate.Year))
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
                    day = (currentDate.Day + increment) - HireDate.Day;
                    increment = 1;
                }
                else
                {
                    day = currentDate.Day - HireDate.Day;
                }

                if ((HireDate.Month + increment) > currentDate.Month)
                {
                    month = (currentDate.Month + 12) - (HireDate.Month + increment);
                    increment = 1;
                }
                else
                {
                    month = (currentDate.Month) - (HireDate.Month + increment);
                    increment = 0;
                }

                year = currentDate.Year - (HireDate.Year + increment);

                var yearsString = (year > 4 || year == 0) ? "л." : "г.";
                var durationString = $"{year} {yearsString} {month} мес. {day} дн.";

                return durationString;
            }
        }
    }
}