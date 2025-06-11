namespace KOP.Common.Dtos.GradeDtos
{
    public class QualificationDto
    {
        public int Id { get; set; }
        public int GradeId { get; set; }
        public bool IsFinalized { get; set; }
        public string? Conclusion { get; set; }

        public DateOnly CurrentJobStartDate { get; set; }
        public DateOnly CurrentStatusDate { get; set; }

        public int CurrentExperienceYears { get; set; }
        public int CurrentExperienceMonths { get; set; }

        public string CurrentJobPositionName { get; set; }
        public string EmploymentContarctTerminations { get; set; }
        public string QualificationResult { get; set; }

        public List<PreviousJobDto> PreviousJobs { get; set; }
        public List<HigherEducationDto> HigherEducations { get; set; }
    }
}