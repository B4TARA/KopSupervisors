namespace KOP.Common.Dtos.GradeDtos
{
    public class TrainingEventDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string Competence { get; set; }
        public int UserId { get; set; }
        public int GradeId { get; set; }
    }
}