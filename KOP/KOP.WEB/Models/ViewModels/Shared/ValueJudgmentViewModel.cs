using KOP.Common.DTOs.GradeDTOs;
using System.ComponentModel.DataAnnotations;

namespace KOP.WEB.Models.ViewModels.Shared
{
    public class ValueJudgmentViewModel
    {
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public int GradeId { get; set; }
        public ValueJudgmentDTO ValueJudgment { get; set; } = new();
    }
}