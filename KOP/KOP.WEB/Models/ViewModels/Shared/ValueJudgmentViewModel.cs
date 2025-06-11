namespace KOP.WEB.Models.ViewModels.Shared
{
    public class ValueJudgmentViewModel
    {
        public int Id { get; set; }
        public int GradeId { get; set; }
        public int SelectedUserId { get; set; }
        public string Strengths { get; set; }
        public string BehaviorToCorrect { get; set; }
        public string RecommendationsForDevelopment { get; set; }
        public bool IsFinalized { get; set; }

        public bool ViewAccess { get; set; }
        public bool EditAccess { get; set; }
    }
}