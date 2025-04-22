namespace KOP.DAL.Entities
{
    public class TrainingEvent
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string Competence { get; set; }

        public Grade Grade { get; set; }
        public int GradeId { get; set; }

        public DateOnly DateOfCreation { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    }
}