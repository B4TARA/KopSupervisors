using KOP.Common.Enums;

namespace KOP.Common.DTOs.AssessmentDTOs
{
    public class AssessmentDTO
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public int EmployeeId { get; set; }
        public string AssessmentTypeName { get; set; } = string.Empty;
        public SystemStatuses SystemStatus { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public bool IsActiveAssessment { get; set; }
        public int AverageValue { get; set; }
        public List<AssessmentResultDTO> AssessmentResults { get; set; } = new();
    }
}