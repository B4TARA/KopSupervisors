namespace KOP.Common.DTOs.GradeDTOs
{
    public class TrainingEventDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string Competence { get; set; }
        public int EmployeeId { get; set; }
        public int GradeId { get; set; }
    }
}