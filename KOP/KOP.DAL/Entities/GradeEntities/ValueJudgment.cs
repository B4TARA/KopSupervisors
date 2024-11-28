using System.ComponentModel.DataAnnotations;

namespace KOP.DAL.Entities.GradeEntities
{
    public class ValueJudgment
    {
        [Key]
        public int Id { get; set; } // Id оценочного суждения

        [Required]
        public string Strengths { get; set; } // Сильные стороны сотрудника

        [Required]
        public string BehaviorToCorrect { get; set; } // Поведение, трубующее корректировки

        [Required]
        public string RecommendationsForDevelopment { get; set; } // Рекомендации к развитию



        public Employee Employee { get; set; } // Сотрудник, к которому относится данный стратегический проекта
        public int EmployeeId { get; set; }



        public Grade Grade { get; set; } // Оценка, к которой относится данный стратегический проекта
        public int GradeId { get; set; }



        public DateOnly DateOfCreation { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    }
}