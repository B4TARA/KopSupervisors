using System.ComponentModel.DataAnnotations;

namespace KOP.Common.Dtos.GradeDtos
{
    public class StrategicTaskDto
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина должна быть от 3 до 50 символов")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина должна быть от 3 до 50 символов")]
        public string Purpose { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [Range(typeof(DateTime), "2023-01-01", "2025-12-31", ErrorMessage = "Дата должна быть в пределах от 1 января 2023 до 31 декабря 2025")]
        public DateTime PlanDateTime { get; set; }
        public DateOnly PlanDate => DateOnly.FromDateTime(PlanDateTime);

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [Range(typeof(DateTime), "2023-01-01", "2025-12-31", ErrorMessage = "Дата должна быть в пределах от 1 января 2023 до 31 декабря 2025")]
        public DateTime FactDateTime { get; set; }
        public DateOnly FactDate => DateOnly.FromDateTime(FactDateTime);

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина должна быть от 3 до 50 символов")]
        public string PlanResult { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина должна быть от 3 до 50 символов")]
        public string FactResult { get; set; }

        public string? Remark { get; set; }
    }
}