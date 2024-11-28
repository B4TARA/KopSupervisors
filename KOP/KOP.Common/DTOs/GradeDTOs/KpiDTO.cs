namespace KOP.Common.DTOs.GradeDTOs
{
    public class KpiDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateOnly PeriodStartDate { get; set; }
        public DateOnly PeriodEndDate { get; set; }
        public int CompletionPercentage { get; set; }
        public string CalculationMethod { get; set; }
        public int EmployeeId { get; set; }
        public int GradeId { get; set; }
    }
}