using System.ComponentModel.DataAnnotations;

namespace KOP.Common.Dtos
{
    public class MarkDto
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public int PercentageValue { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string Period { get; set; }
    }
}