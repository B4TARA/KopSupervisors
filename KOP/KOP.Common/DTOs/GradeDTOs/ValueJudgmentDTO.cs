namespace KOP.Common.DTOs.GradeDTOs
{
    public class ValueJudgmentDTO
    {
        public int Id { get; set; }
        public string Strengths { get; set; }
        public string BehaviorToCorrect { get; set; }
        public string RecommendationsForDevelopment { get; set; }
        public int EmployeeId { get; set; }
        public int GradeId { get; set; }
    }
}