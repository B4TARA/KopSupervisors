namespace KOP.Common.Dtos.GradeDtos
{
    public class KpiDto
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public DateTime PeriodStartDateTime { get; set; } = DateTime.Today;
        public DateOnly PeriodStartDate => DateOnly.FromDateTime(PeriodStartDateTime);
        public DateTime PeriodEndDateTime { get; set; } = DateTime.Today;
        public DateOnly PeriodEndDate => DateOnly.FromDateTime(PeriodEndDateTime);
        public string CompletionPercentage { get; set; }
        public string CalculationMethod { get; set; }
    }
}