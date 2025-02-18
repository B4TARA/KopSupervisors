using KOP.Common.Enums;

namespace KOP.Common.Dtos.AssessmentDtos
{
    public class AssessmentSummaryDto
    {
        public List<AssessmentInterpretationDto> AssessmentTypeInterpretations { get; set; } = new();
        public AssessmentInterpretationDto? AverageAssessmentInterpretation { get; set; }
        public List<AssessmentResultValueDto> AverageAssessmentResultValues { get; set; } = new();
        public List<AssessmentResultValueDto> SelfAssessmentResultValues { get; set; } = new();
        public List<AssessmentResultValueDto> SupervisorAssessmentResultValues { get; set; } = new();
        public List<AssessmentMatrixElementDto> Elements { get; set; } = new();
        public List<IGrouping<int, AssessmentMatrixElementDto>> ElementsByRow { get; set; } = new();
        public int SumResult { get; set; }
        public double AverageResult { get; set; }
        public bool IsFinalized { get; set; }
    }
}