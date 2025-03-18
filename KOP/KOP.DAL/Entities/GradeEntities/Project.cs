namespace KOP.DAL.Entities.GradeEntities
{
    public class Project
    {
        public int Id { get; set; }
        public string UserRole { get; set; }
        public string Name { get; set; }
        public string Stage { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int SuccessRate { get; set; }
        public int AverageKpi { get; set; }
        public double SP { get; set; }

        public Grade Grade { get; set; }
        public int GradeId { get; set; }

        public DateOnly DateOfCreation { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    }
}