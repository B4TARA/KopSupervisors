namespace KOP.Common.Dtos.GradeDtos
{
    public class QualificationDto
    {
        public int? Id { get; set; }
        public DateTime CurrentStatusDateTime { get; set; } = DateTime.Today;
        public DateOnly CurrentStatusDate => DateOnly.FromDateTime(CurrentStatusDateTime);
        public int CurrentExperienceYears { get; set; }
        public int CurrentExperienceMonths { get; set; }
        public DateTime CurrentJobStartDateTime { get; set; } = DateTime.Today;
        public DateOnly CurrentJobStartDate => DateOnly.FromDateTime(CurrentJobStartDateTime);
        public string CurrentJobPositionName { get; set; }
        public string EmploymentContarctTerminations { get; set; }
        public string QualificationResult { get; set; }

        public List<PreviousJobDto> PreviousJobs { get; set; } = new();
        public List<HigherEducationDto> HigherEducations { get; set; } = new();
    }
}