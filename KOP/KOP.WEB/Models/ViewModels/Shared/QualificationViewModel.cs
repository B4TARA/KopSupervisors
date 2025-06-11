using KOP.Common.Dtos.GradeDtos;

namespace KOP.WEB.Models.ViewModels.Shared
{
    public class QualificationViewModel
    {
        public int Id { get; set; }
        public int GradeId { get; set; }
        public bool IsFinalized { get; set; }
        public string Conclusion { get; set; }
        public int SelectedUserId { get; set; }
        public string SelectedUserFullName { get; set; }

        public DateOnly CurrentStatusDate { get; set; }
        public DateOnly CurrentJobStartDate { get; set; }

        public int CurrentExperienceYears { get; set; }
        public int CurrentExperienceMonths { get; set; }

        public string CurrentJobPositionName { get; set; }
        public string EmploymentContarctTerminations { get; set; }
        public string QualificationResult { get; set; }

        public List<PreviousJobDto> PreviousJobs { get; set; } = new();
        public List<HigherEducationDto> HigherEducations { get; set; } = new();

        public bool ViewAccess { get; set; }
        public bool EditAccess { get; set; }
        public bool ConclusionEditAccess { get; set; }
    }
}