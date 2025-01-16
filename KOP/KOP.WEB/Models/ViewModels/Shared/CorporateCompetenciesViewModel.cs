using KOP.Common.DTOs.AssessmentDTOs;

namespace KOP.WEB.Models.ViewModels.Shared
{
    public class CorporateCompetenciesViewModel
    {
        public string? Conclusion { get; set; }
        public List<AssessmentResultDTO> LastCompletedAssessmentResults { get; set; } = new();
    }
}