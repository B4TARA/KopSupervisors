using System.ComponentModel.DataAnnotations;

namespace KOP.Common.Dtos.GradeDtos
{
    public class KpiDto
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public DateTime PeriodStartDateTime { get; set; }
        public DateOnly PeriodStartDate => DateOnly.FromDateTime(PeriodStartDateTime);

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public DateTime PeriodEndDateTime { get; set; }
        public DateOnly PeriodEndDate => DateOnly.FromDateTime(PeriodEndDateTime);

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public int CompletionPercentage { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string CalculationMethod { get; set; }
    }
}