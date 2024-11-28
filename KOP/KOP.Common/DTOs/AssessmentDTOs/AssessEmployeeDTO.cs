namespace KOP.Common.DTOs.AssessmentDTOs
{
    public class AssessEmployeeDTO
    {
        public int AssessmentResultId { get; set; }
        public List<string> ResultValues { get; set; } = new();
    }
}