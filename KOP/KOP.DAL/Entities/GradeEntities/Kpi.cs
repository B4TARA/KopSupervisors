namespace KOP.DAL.Entities.GradeEntities
{
    public class Kpi
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateOnly PeriodStartDate { get; set; }
        public DateOnly PeriodEndDate { get; set; }
        public int CompletionPercentage { get; set; }
        public string CalculationMethod { get; set; }
        public Grade Grade { get; set; }
        public int GradeId { get; set; }
        public DateOnly DateOfCreation { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    }
}