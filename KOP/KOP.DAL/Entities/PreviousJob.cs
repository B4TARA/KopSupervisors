namespace KOP.DAL.Entities
{
    public class PreviousJob
    {
        public int Id { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string OrganizationName { get; set; }
        public string PositionName { get; set; }

        public Qualification Qualification { get; set; }
        public int QualificationId { get; set; }

        public DateOnly DateOfCreation { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    }
}