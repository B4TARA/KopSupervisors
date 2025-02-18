namespace KOP.Common.Dtos.GradeDtos
{
    public class KpiDto
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public DateTime PeriodStartDateTime { get; set; }
        public DateOnly PeriodStartDate => DateOnly.FromDateTime(PeriodStartDateTime);
        public DateTime PeriodEndDateTime { get; set; }
        public DateOnly PeriodEndDate => DateOnly.FromDateTime(PeriodEndDateTime);
        public int CompletionPercentage { get; set; }
        public string CalculationMethod { get; set; }
    }
}