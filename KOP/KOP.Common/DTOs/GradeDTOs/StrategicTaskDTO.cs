namespace KOP.Common.Dtos.GradeDtos
{
    public class StrategicTaskDto
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Purpose { get; set; }
        public DateTime PlanDateTime { get; set; } = DateTime.Today;
        public DateOnly PlanDate => DateOnly.FromDateTime(PlanDateTime);
        public DateTime FactDateTime { get; set; } = DateTime.Today;
        public DateOnly FactDate => DateOnly.FromDateTime(FactDateTime);
        public string PlanResult { get; set; }
        public string FactResult { get; set; }
        public string? Remark { get; set; }
    }
}