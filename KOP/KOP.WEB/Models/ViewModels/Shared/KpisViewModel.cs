using System.ComponentModel.DataAnnotations;
using KOP.Common.Dtos.GradeDtos;

namespace KOP.WEB.Models.ViewModels.Shared
{
    public class KpisViewModel
    {
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public int GradeId { get; set; }
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина должна быть от 3 до 50 символов")]
        public string? Conclusion { get; set; }
        public List<KpiDto> Kpis { get; set; } = new();
    }
}