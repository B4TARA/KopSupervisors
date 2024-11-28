using KOP.Common.Enums;

namespace KOP.Common.DTOs.AssessmentDTOs
{
    public class AssessmentResultDTO
    {
        public int Id { get; set; }
        public int AssessmentId { get; set; }

        public string TypeName { get; set; }
        public int Sum { get; set; }
        public bool IsPassed { get; set; }
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
        public SystemStatuses SystemStatus { get; set; }

        public EmployeeDTO Judge { get; set; } = new();
        public EmployeeDTO Judged { get; set; } = new();

        public List<AssessmentResultValueDTO> AverageValues { get; set; } = new();
        public List<AssessmentResultValueDTO> Values { get; set; } = new();
        public List<AssessmentMatrixElementDTO> Elements { get; set; } = new();
        public List<IGrouping<int, AssessmentMatrixElementDTO>> ElementsByRow { get; set; } = new();   

        public double AverageResult { get; set; }
        public int PlanValue { get; set; }
    }
}