using KOP.Common.Dtos.AssessmentDtos;

namespace KOP.WEB.Models.ViewModels.Shared
{
    public class ManagmentCompetenciesViewModel
    {
        public string? Conclusion { get; set; }
        public AssessmentSummaryDto AssessmentSummaryDto { get; set; } = new();
    }
}