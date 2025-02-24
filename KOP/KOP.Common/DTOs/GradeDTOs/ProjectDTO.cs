namespace KOP.Common.Dtos.GradeDtos
{
    public class ProjectDto
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string SupervisorSspName { get; set; }
        public string Stage { get; set; }
        public DateTime StartDateTime { get; set; } = DateTime.Today;
        public DateOnly StartDate => DateOnly.FromDateTime(StartDateTime);
        public DateTime EndDateTime { get; set; } = DateTime.Today;
        public DateOnly EndDate => DateOnly.FromDateTime(EndDateTime);
        public DateTime CurrentStatusDateTime { get; set; } = DateTime.Today;
        public DateOnly CurrentStatusDate => DateOnly.FromDateTime(CurrentStatusDateTime);
        public int PlanStages { get; set; }
        public int FactStages { get; set; }
        public int SPn { get; set; }
    }
}