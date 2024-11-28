using KOP.Common.DTOs.AssessmentDTOs;

namespace KOP.WEB.Models.ViewModels.Shared
{
    public class ManagmentCompetenciesViewModel
    {
        public int EmployeeId { get; set; }
        public int GradeId { get; set; }
        public string? ManagmentCompetenciesConclusion { get; set; }
        public AssessmentDTO LastAssessment { get; set; } = new();
    }
}