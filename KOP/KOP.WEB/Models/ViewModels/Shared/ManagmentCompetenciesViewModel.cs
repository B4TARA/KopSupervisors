using KOP.Common.Dtos.AssessmentDtos;

namespace KOP.WEB.Models.ViewModels.Shared
{
    public class ManagmentCompetenciesViewModel
    {
        public string? Conclusion { get; set; }
        public List<AssessmentResultDto> LastCompletedAssessmentResults { get; set; } = new();
    }
}