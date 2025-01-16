using System.ComponentModel.DataAnnotations;
using System.Net;

namespace KOP.Common.DTOs.GradeDTOs
{
    public class QualificationDTO
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина должна быть от 3 до 50 символов")]
        public string SupervisorSspName { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина должна быть от 3 до 50 символов")]
        public string Link { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина должна быть от 3 до 50 символов")]
        public string HigherEducation { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина должна быть от 3 до 50 символов")]
        public string Speciality { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина должна быть от 3 до 50 символов")]
        public string QualificationResult { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [Range(typeof(DateTime), "2023-01-01", "2025-12-31", ErrorMessage = "Дата должна быть в пределах от 1 января 2023 до 31 декабря 2025")]
        public DateTime StartDateTime { get; set; }
        public DateOnly StartDate => DateOnly.FromDateTime(StartDateTime);

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [Range(typeof(DateTime), "2023-01-01", "2025-12-31", ErrorMessage = "Дата должна быть в пределах от 1 января 2023 до 31 декабря 2025")]
        public DateTime EndDateTime { get; set; }
        public DateOnly EndDate => DateOnly.FromDateTime(EndDateTime);

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина должна быть от 3 до 50 символов")]
        public string AdditionalEducation { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [Range(typeof(DateTime), "2023-01-01", "2025-12-31", ErrorMessage = "Дата должна быть в пределах от 1 января 2023 до 31 декабря 2025")]
        public DateTime CurrentStatusDateTime { get; set; }
        public DateOnly CurrentStatusDate => DateOnly.FromDateTime(CurrentStatusDateTime);

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [Range(1, 50, ErrorMessage = "Опыт работы должен быть от 1 до 50 лет")]
        public int CurrentExperienceYears { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [Range(1, 12, ErrorMessage = "Количество месяцев может быть от 1 до 12")]
        public int CurrentExperienceMonths { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [Range(typeof(DateTime), "2023-01-01", "2025-12-31", ErrorMessage = "Дата должна быть в пределах от 1 января 2023 до 31 декабря 2025")]
        public DateTime CurrentJobStartDateTime { get; set; }
        public DateOnly CurrentJobStartDate => DateOnly.FromDateTime(CurrentJobStartDateTime);

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина должна быть от 3 до 50 символов")]
        public string CurrentJobPositionName { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина должна быть от 3 до 50 символов")]
        public string EmploymentContarctTerminations { get; set; }

        public List<PreviousJobDTO> PreviousJobs = new();
    }
}