using KOP.Common.DTOs.AssessmentDTOs;

namespace KOP.WEB.Models.ViewModels.Employee
{
    public class SelfAssessmentViewModel
    {
        public AssessmentResultDTO SelfAssessmentResult { get; set; } = new();
    }
}