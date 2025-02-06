using System.ComponentModel.DataAnnotations;

namespace KOP.Common.Dtos.GradeDtos
{
    public class QualificationDto
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string SupervisorSspName { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string Link { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string HigherEducation { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string Speciality { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string QualificationResult { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public DateTime StartDateTime { get; set; }
        public DateOnly StartDate => DateOnly.FromDateTime(StartDateTime);

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public DateTime EndDateTime { get; set; }
        public DateOnly EndDate => DateOnly.FromDateTime(EndDateTime);

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string AdditionalEducation { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public DateTime CurrentStatusDateTime { get; set; }
        public DateOnly CurrentStatusDate => DateOnly.FromDateTime(CurrentStatusDateTime);

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public int CurrentExperienceYears { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [Range(1, 12, ErrorMessage = "Количество месяцев может быть от 1 до 12")]
        public int CurrentExperienceMonths { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public DateTime CurrentJobStartDateTime { get; set; }
        public DateOnly CurrentJobStartDate => DateOnly.FromDateTime(CurrentJobStartDateTime);

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string CurrentJobPositionName { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string EmploymentContarctTerminations { get; set; }

        public List<PreviousJobDto> PreviousJobs = new();
    }
}