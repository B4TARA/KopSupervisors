namespace KOP.DAL.Entities.GradeEntities
{
    public class Qualification
    {
        public int Id { get; set; }
        public DateOnly CurrentStatusDate { get; set; }
        public int CurrentExperienceYears { get; set; }
        public int CurrentExperienceMonths { get; set; }
        public DateOnly CurrentJobStartDate { get; set; }
        public string CurrentJobPositionName { get; set; }
        public string EmploymentContarctTerminations { get; set; }
        public string QualificationResult { get; set; }

        public Grade Grade { get; set; }
        public int GradeId { get; set; }

        public List<HigherEducation> HigherEducations { get; set; } = new();
        public List<PreviousJob> PreviousJobs { get; set; } = new();

        public DateOnly DateOfCreation { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    }
}