using KOP.Common.Enums;

namespace KOP.Common.Dtos.AssessmentDtos
{
    public class AssessmentResultDto
    {
        public int Id { get; set; }
        public int AssessmentId { get; set; }

        public string TypeName { get; set; }
        public int Sum { get; set; }
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
        public int? AssignedBy { get; set; }
        public SystemStatuses SystemStatus { get; set; }
        public AssessmentResultTypes Type { get; set; }
        public string HtmlClassName { get; set; } = string.Empty;

        public UserDto Judge { get; set; } = new();
        public UserDto Judged { get; set; } = new();

        public List<AssessmentResultValueDto> AverageValues { get; set; } = new();
        public List<AssessmentResultValueDto> Values { get; set; } = new();
        public List<AssessmentMatrixElementDto> Elements { get; set; } = new();
        public List<IGrouping<int, AssessmentMatrixElementDto>> ElementsByRow { get; set; } = new();

        public double AverageResult { get; set; }
        public int PlanValue { get; set; }
    }
}