using System.ComponentModel.DataAnnotations;

namespace KOP.Common.DTOs
{
    public class MarkDTO
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [Range(1, 100, ErrorMessage = "Процентное значени должно быть от 1 до 100")]
        public int PercentageValue { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина должна быть от 3 до 50 символов")]
        public string Period { get; set; }
    }
}