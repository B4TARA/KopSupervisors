using KOP.Common.DTOs.GradeDTOs;

namespace KOP.WEB.Models.ViewModels.Shared
{
    public class ValueJudgmentViewModel
    {
        public int GradeId { get; set; }
        public ValueJudgmentDTO ValueJudgment { get; set; } = new();
    }
}