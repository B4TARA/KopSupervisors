using KOP.Common.DTOs.AssessmentDTOs;

namespace KOP.WEB.Models.ViewModels.Shared
{
    public class CorporateCompetenciesViewModel
    {
        public int EmployeeId { get; set; }
        public int GradeId { get; set; }
        public string? CorporateCompetenciesConclusion { get; set; }
        public AssessmentDTO LastAssessment { get; set; } = new();
    }
}