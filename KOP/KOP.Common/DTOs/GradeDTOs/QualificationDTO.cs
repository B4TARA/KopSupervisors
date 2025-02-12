namespace KOP.Common.Dtos.GradeDtos
{
    public class QualificationDto
    {
        public int? Id { get; set; }
        public string SupervisorSspName { get; set; }
        public string Link { get; set; }
        public string HigherEducation { get; set; }
        public string Speciality { get; set; }
        public string QualificationResult { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateOnly StartDate => DateOnly.FromDateTime(StartDateTime);
        public DateTime EndDateTime { get; set; }
        public DateOnly EndDate => DateOnly.FromDateTime(EndDateTime);
        public string AdditionalEducation { get; set; }
        public DateTime CurrentStatusDateTime { get; set; }
        public DateOnly CurrentStatusDate => DateOnly.FromDateTime(CurrentStatusDateTime);
        public int CurrentExperienceYears { get; set; }
        public int CurrentExperienceMonths { get; set; }
        public DateTime CurrentJobStartDateTime { get; set; }
        public DateOnly CurrentJobStartDate => DateOnly.FromDateTime(CurrentJobStartDateTime);
        public string CurrentJobPositionName { get; set; }
        public string EmploymentContarctTerminations { get; set; }

        public List<PreviousJobDto> PreviousJobs { get; set; } = new();
    }
}