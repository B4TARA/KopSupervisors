namespace KOP.DAL.Entities
{
    public class Project
    {
        public int Id { get; set; }
        public string UserRole { get; set; }
        public string Name { get; set; }
        public string Stage { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string SuccessRate { get; set; }
        public string AverageKpi { get; set; }
        public string SP { get; set; }

        public int GradeId { get; set; }
        public Grade Grade { get; set; }

        public DateOnly DateOfCreation { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    }
}