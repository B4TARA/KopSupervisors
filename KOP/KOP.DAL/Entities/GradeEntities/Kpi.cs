using System.ComponentModel.DataAnnotations;

namespace KOP.DAL.Entities.GradeEntities
{
    public class Kpi
    {
        [Key]
        public int Id { get; set; } // Id КПЭ

        [Required]
        public string Name { get; set; } // Наименование КПЭ

        [Required]
        public DateOnly PeriodStartDate { get; set; } // Период с

        [Required]
        public DateOnly PeriodEndDate { get; set; } // Период по

        [Required]
        public int CompletionPercentage { get; set; } // Процент выполнения

        [Required]
        public string CalculationMethod { get; set; } // Методика расчета



        public Grade Grade { get; set; } // Оценка, к которой относится данный КПЭ
        public int GradeId { get; set; }



        public DateOnly DateOfCreation { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    }
}
