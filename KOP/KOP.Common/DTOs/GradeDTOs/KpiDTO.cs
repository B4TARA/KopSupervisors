using System.ComponentModel.DataAnnotations;

namespace KOP.Common.DTOs.GradeDTOs
{
    public class KpiDTO
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина должна быть от 3 до 50 символов")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [Range(typeof(DateTime), "2023-01-01", "2025-12-31", ErrorMessage = "Дата должна быть в пределах от 1 января 2023 до 31 декабря 2025")]
        public DateTime PeriodStartDateTime { get; set; }
        public DateOnly PeriodStartDate => DateOnly.FromDateTime(PeriodStartDateTime);

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [Range(typeof(DateTime), "2023-01-01", "2025-12-31", ErrorMessage = "Дата должна быть в пределах от 1 января 2023 до 31 декабря 2025")]
        public DateTime PeriodEndDateTime { get; set; }
        public DateOnly PeriodEndDate => DateOnly.FromDateTime(PeriodEndDateTime);

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [Range(1, 100, ErrorMessage = "Процент выполнения должен быть от 1 до 100 %")]
        public int CompletionPercentage { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина должна быть от 3 до 50 символов")]
        public string CalculationMethod { get; set; }
    }
}