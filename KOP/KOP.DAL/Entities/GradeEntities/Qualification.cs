using System.ComponentModel.DataAnnotations;

namespace KOP.DAL.Entities.GradeEntities
{
    public class Qualification
    {
        [Key]
        public int Id { get; set; }
        public string SupervisorSspName { get; set; } = "ФИО рук-ля ССП";
        public string Link { get; set; } = "ссылка на ЛПА";
        public string HigherEducation { get; set; } = "УО";
        public string Speciality { get; set; } = "_";
        public string QualificationResult { get; set; } = "_";
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string AdditionalEducation { get; set; } = "_";
        public DateOnly CurrentStatusDate { get; set; }
        public int CurrentExperienceYears { get; set; }
        public int CurrentExperienceMonths { get; set; }
        public DateOnly CurrentJobStartDate { get; set; }
        public string CurrentJobPositionName { get; set; } = "наименование текущей должности";
        public string EmploymentContarctTerminations { get; set; } = "Отсутствуют";

        public Grade Grade { get; set; }
        public int GradeId { get; set; }

        public List<PreviousJob> PreviousJobs { get; set; } = new ();

        public DateOnly DateOfCreation { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    }
}