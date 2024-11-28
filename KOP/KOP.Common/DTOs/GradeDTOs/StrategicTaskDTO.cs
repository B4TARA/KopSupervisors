namespace KOP.Common.DTOs.GradeDTOs
{
    public class StrategicTaskDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Purpose { get; set; }
        public DateOnly PlanDate { get; set; }
        public DateOnly FactDate { get; set; }
        public string PlanResult { get; set; }
        public string FactResult { get; set; }
        public string? Remark { get; set; }
        public int GradeId { get; set; }
    }
}