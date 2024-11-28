using KOP.Common.DTOs.AssessmentDTOs;

namespace KOP.WEB.Models.ViewModels.Supervisor
{
    public class EmployeeAssessmentViewModel
    {
        public AssessmentDTO Assessment { get; set; } = new();
        public AssessmentResultDTO? SupervisorAssessmentResult { get; set; }
    }
}