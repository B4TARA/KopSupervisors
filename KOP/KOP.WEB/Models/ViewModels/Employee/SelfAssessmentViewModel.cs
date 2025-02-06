using KOP.Common.Dtos.AssessmentDtos;

namespace KOP.WEB.Models.ViewModels.Employee
{
    public class SelfAssessmentViewModel
    {
        public AssessmentResultDto SelfAssessmentResult { get; set; } = new();
    }
}