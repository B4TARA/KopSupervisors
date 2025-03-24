using KOP.Common.Dtos.AssessmentDtos;
using KOP.Common.Enums;

namespace KOP.WEB.Models.ViewModels.Admin
{
    public class AssessmentMatrixViewModel
    {
        public SystemAssessmentTypes SystemAssessmentType { get; set; }
        public List<AssessmentMatrixElementDto> Elements { get; set; } = new();
    }
}