using KOP.Common.Dtos.AssessmentDtos;
using KOP.Common.Dtos.UserDtos;
using KOP.Common.Enums;

namespace KOP.Common.Dtos.AssessmentResultDtos
{
    public class AssessmentResultDto
    {
        public int Id { get; set; }
        public int AssessmentTypeId { get; set; }

        public string TypeName { get; set; }
        public int Sum { get; set; }
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
        public int? AssignedBy { get; set; }
        public SystemStatuses SystemStatus { get; set; }
        public AssessmentResultTypes Type { get; set; }
        public string HtmlClassName { get; set; } = string.Empty;

        public UserExtendedDto Judge { get; set; } = new();
        public UserExtendedDto Judged { get; set; } = new();

        public List<AssessmentResultValueDto> Values { get; set; } = new();
        public List<AssessmentMatrixElementDto> Elements { get; set; } = new();
        public List<IGrouping<int, AssessmentMatrixElementDto>> ElementsByRow { get; set; } = new();
    }
}