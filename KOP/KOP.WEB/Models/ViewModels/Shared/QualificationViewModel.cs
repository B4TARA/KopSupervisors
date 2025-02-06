using KOP.Common.Dtos.GradeDtos;

namespace KOP.WEB.Models.ViewModels.Shared
{
    public class QualificationViewModel
    {
        public int GradeId { get; set; }
        public string? Conclusion { get; set; }
        public QualificationDto Qualification { get; set; } = new();
    }
}