using System.ComponentModel.DataAnnotations;

namespace KOP.Common.DTOs.GradeDTOs
{
    public class ProjectDTO
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина должна быть от 3 до 50 символов")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина должна быть от 3 до 50 символов")]
        public string SupervisorSspName { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина должна быть от 3 до 50 символов")]
        public string Stage { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [Range(typeof(DateTime), "2023-01-01", "2025-12-31", ErrorMessage = "Дата должна быть в пределах от 1 января 2023 до 31 декабря 2025")]
        public DateTime StartDateTime { get; set; }
        public DateOnly StartDate => DateOnly.FromDateTime(StartDateTime);

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [Range(typeof(DateTime), "2023-01-01", "2025-12-31", ErrorMessage = "Дата должна быть в пределах от 1 января 2023 до 31 декабря 2025")]
        public DateTime EndDateTime { get; set; }
        public DateOnly EndDate => DateOnly.FromDateTime(EndDateTime);

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [Range(typeof(DateTime), "2023-01-01", "2025-12-31", ErrorMessage = "Дата должна быть в пределах от 1 января 2023 до 31 декабря 2025")]
        public DateTime CurrentStatusDateTime { get; set; }
        public DateOnly CurrentStatusDate => DateOnly.FromDateTime(CurrentStatusDateTime);

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [Range(1, 10, ErrorMessage = "Количество этапов должно быть от 1 до 10")]
        public int PlanStages { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [Range(1, 10, ErrorMessage = "Количество этапов должно быть от 1 до 10")]
        public int FactStages { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [Range(1, 100, ErrorMessage = "Коэффициент реализации должен быть от 1 до 100 %")]
        public int SPn { get; set; }
    }
}