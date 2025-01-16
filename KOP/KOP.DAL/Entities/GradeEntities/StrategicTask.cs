using System.ComponentModel.DataAnnotations;

namespace KOP.DAL.Entities.GradeEntities
{
    public class StrategicTask
    {
        [Key]
        public int Id { get; set; } // id стратегической задачи

        [Required]
        public string Name { get; set; } // Название стратегической задачи

        [Required]
        public string Purpose { get; set; } // Цель стратегической задачи

        [Required]
        public DateOnly PlanDate { get; set; } // План стратегической задачи (Дата)

        [Required]
        public DateOnly FactDate { get; set; } // Факт стратегической задачи (Дата)

        [Required]
        public string PlanResult { get; set; } // План стратегической задачи

        [Required]
        public string FactResult { get; set; } // Факт стратегической задачи

        public string? Remark { get; set; } // Примечание ( в случае несоответствия планового и фактического срока реализации)



        public Grade Grade { get; set; } // Оценка, к которой относится данная стратегическая задача
        public int GradeId { get; set; }



        public DateOnly DateOfCreation { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    }
}