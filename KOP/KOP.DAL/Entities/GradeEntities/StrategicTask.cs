namespace KOP.DAL.Entities.GradeEntities
{
    public class StrategicTask
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Purpose { get; set; }
        public DateOnly PlanDate { get; set; }
        public DateOnly FactDate { get; set; }
        public string PlanResult { get; set; }
        public string FactResult { get; set; }
        public string? Remark { get; set; }

        public Grade Grade { get; set; }
        public int GradeId { get; set; }

        public DateOnly DateOfCreation { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    }
}