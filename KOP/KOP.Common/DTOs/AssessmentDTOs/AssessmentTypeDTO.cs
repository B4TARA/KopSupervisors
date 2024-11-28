namespace KOP.Common.DTOs.AssessmentDTOs
{
    public class AssessmentTypeDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int PlanValue { get; set; }
        public int EmployeeId { get; set; }

        public List<AssessmentDTO> Assessments { get; set; } = new();
    }
}