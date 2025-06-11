namespace KOP.Common.Dtos.GradeDtos
{
    public class ValueJudgmentDto
    {
        public int Id { get; set; }
        public int GradeId { get; set; }
        public string Strengths { get; set; }
        public string BehaviorToCorrect { get; set; }
        public string RecommendationsForDevelopment { get; set; }
        public bool IsFinalized { get; set; }
    }
}