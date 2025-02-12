namespace KOP.Common.Dtos.AssessmentDtos
{
    public class AssessmentSummaryDto
    {
        public List<AssessmentResultValueDto> AverageAssessmentResultValues { get; set; } = new();
        public List<AssessmentResultValueDto> SelfAssessmentResultValues { get; set; } = new();
        public List<AssessmentResultValueDto> SupervisorAssessmentResultValues { get; set; } = new();
        public List<AssessmentMatrixElementDto> Elements { get; set; } = new();
        public List<IGrouping<int, AssessmentMatrixElementDto>> ElementsByRow { get; set; } = new();

        public double AverageResult { get; set; }
        public int PlanValue { get; set; }
    }
}