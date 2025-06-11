using KOP.Common.Dtos.AssessmentResultDtos;
using KOP.Common.Enums;

namespace KOP.Common.Dtos.AssessmentDtos
{
    public class AssessmentDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string AssessmentTypeName { get; set; }
        public SystemAssessmentTypes SystemAssessmentType { get; set; }
        public SystemStatuses SystemStatus { get; set; }
        public bool IsActiveAssessment { get; set; }
        public int SumValue { get; set; }
        public int AverageValue { get; set; }
        public List<AssessmentResultDto> AllAssessmentResults { get; set; } = new();
        public List<AssessmentResultDto> CompletedAssessmentResults { get; set; } = new();
        public AssessmentInterpretationDto? AverageAssessmentInterpretation { get; set; }
    }
}