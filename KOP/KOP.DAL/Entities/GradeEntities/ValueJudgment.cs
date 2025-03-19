namespace KOP.DAL.Entities.GradeEntities
{
    public class ValueJudgment
    {
        public int Id { get; set; }
        public string Strengths { get; set; }
        public string BehaviorToCorrect { get; set; }
        public string RecommendationsForDevelopment { get; set; }

        public Grade Grade { get; set; }
        public int GradeId { get; set; }

        public DateOnly DateOfCreation { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    }
}