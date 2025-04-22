namespace KOP.Common.Dtos.GradeDtos
{
    public class PreviousJobDto
    {
        public int? Id { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateOnly StartDate => DateOnly.FromDateTime(StartDateTime);
        public DateTime EndDateTime { get; set; }
        public DateOnly EndDate => DateOnly.FromDateTime(EndDateTime);
        public string OrganizationName { get; set; }
        public string PositionName { get; set; }
    }
}