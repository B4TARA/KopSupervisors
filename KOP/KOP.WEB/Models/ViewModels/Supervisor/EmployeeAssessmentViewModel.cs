using KOP.Common.Dtos.AssessmentDtos;

namespace KOP.WEB.Models.ViewModels.Supervisor
{
    public class EmployeeAssessmentViewModel
    {
        public AssessmentDto Assessment { get; set; } = new();
        public AssessmentResultDto? SupervisorAssessmentResult { get; set; }
    }
}