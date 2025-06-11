using KOP.Common.Enums;

namespace KOP.Common.Dtos.AssessmentDtos
{
    public class AssessmentSummaryDto
    {
        public int AssessmentId { get; set; }
        public int UserId { get; set; }
        public SystemAssessmentTypes SystemAssessmentType { get; set; }

        public int MinValue { get; set; }
        public int MaxValue { get; set; }

        public List<AssessmentInterpretationDto> AssessmentTypeInterpretations { get; set; } = new();
        public AssessmentInterpretationDto? AverageAssessmentInterpretation { get; set; }
        public List<AssessmentResultValueDto> AverageValuesByRow { get; set; } = new();

        public bool HasSelfAssessmentResult { get; set; }
        public int SelfAssessmentResultId { get; set; }
        public List<AssessmentResultValueDto> SelfAssessmentResultValues { get; set; } = new();
        public SystemStatuses SelfAssessmentResultSystemStatus { get; set; }

        public List<AssessmentResultValueDto> SupervisorAssessmentResultValues { get; set; } = new();
        public List<AssessmentResultValueDto> ColleaguesAssessmentResultValues { get; set; } = new();
        public List<IGrouping<int, AssessmentMatrixElementDto>> RowsWithElements { get; set; } = new();
        public int SumResult { get; set; }
        public int ColleaguesSumResult { get; set; }
        public double GeneralAverageResult { get; set; }
        public double AverageColleaguesResult { get; set; }
        public double AverageSelfValue { get; set; }
        public double AverageSupervisorValue { get; set; }
        public bool IsFinalized { get; set; }
        public double GetGeneralAverageValue()
        {
            if (AverageValuesByRow.Count > 0)
            {
                return Math.Round(AverageValuesByRow.Average(x => x.Value), 1);
            }
            else
            {
                return 0;
            }
        }
        public double GetGeneralColleaguesValue()
        {
            if (AverageValuesByRow.Count > 0)
            {
                return Math.Round(AverageColleaguesResult / AverageValuesByRow.Count, 1);
            }
            else
            {
                return 0;
            }
        }
    }
}