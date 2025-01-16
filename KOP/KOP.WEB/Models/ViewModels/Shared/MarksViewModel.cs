using KOP.Common.DTOs;
using System.ComponentModel.DataAnnotations;

namespace KOP.WEB.Models.ViewModels.Shared
{
    public class MarksViewModel
    {
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public int GradeId { get; set; }
        public List<MarkTypeDTO> MarkTypes { get; set; } = new();
    }
}