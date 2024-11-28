using KOP.Common.DTOs.GradeDTOs;

namespace KOP.WEB.Models.ViewModels.Shared
{
    public class QualificationViewModel
    {
        public int GradeId { get; set; }
        public string? QualificationConclusion { get; set; }
        public QualificationDTO Qualification { get; set; } = new();
    }
}