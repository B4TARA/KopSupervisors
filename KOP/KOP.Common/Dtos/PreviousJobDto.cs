namespace KOP.Common.Dtos.GradeDtos
{
    public class PreviousJobDto
    {
        public int Id { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string OrganizationName { get; set; }
        public string PositionName { get; set; }
    }
}