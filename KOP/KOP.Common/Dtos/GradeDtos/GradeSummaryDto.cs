namespace KOP.Common.Dtos.GradeDtos
{
    public class GradeSummaryDto
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public DateOnly DateOfCreation { get; set; }
    }
}