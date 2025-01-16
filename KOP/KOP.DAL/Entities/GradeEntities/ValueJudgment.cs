using System.ComponentModel.DataAnnotations;

namespace KOP.DAL.Entities.GradeEntities
{
    public class ValueJudgment
    {
        [Key]
        public int Id { get; set; }
        public string Strengths { get; set; } = "_";
        public string BehaviorToCorrect { get; set; } = "_";
        public string RecommendationsForDevelopment { get; set; } = "_";

        public Grade Grade { get; set; }
        public int GradeId { get; set; }

        public DateOnly DateOfCreation { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    }
}