using System.ComponentModel.DataAnnotations;

namespace KOP.Common.DTOs.GradeDTOs
{
    public class QualificationDTO
    {
        public int Id { get; set; }
        public string SupervisorSspName { get; set; }
        public string Link { get; set; }
        public string HigherEducation { get; set; }
        public string Speciality { get; set; }
        public string QualificationResult { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string AdditionalEducation { get; set; }
        public string CurrentDate { get; set; }
        public string ExperienceMonths { get; set; }
        public string ExperienceYears { get; set; }
        public string PreviousPosition1 { get; set; }
        public string PreviousPosition2 { get; set; }
        public string CurrentPosition { get; set; }
        public string EmploymentContarctTerminations { get; set; }
        public int EmployeeId { get; set; }
        public int GradeId { get; set; }
    }
}