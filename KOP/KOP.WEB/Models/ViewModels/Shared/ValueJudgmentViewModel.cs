using KOP.Common.Dtos.GradeDtos;

namespace KOP.WEB.Models.ViewModels.Shared
{
    public class ValueJudgmentViewModel
    {
        public int GradeId { get; set; }
        public ValueJudgmentDto ValueJudgment { get; set; } = new();
        public bool IsFinalized { get; set; }

        public bool ViewAccess { get; set; }
        public bool EditAccess { get; set; }
    }
}