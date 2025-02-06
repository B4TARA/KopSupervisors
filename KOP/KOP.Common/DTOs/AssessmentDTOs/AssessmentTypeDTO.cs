namespace KOP.Common.Dtos.AssessmentDtos
{
    public class AssessmentTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int PlanValue { get; set; }
        public int UserId { get; set; }

        public List<AssessmentDto> Assessments { get; set; } = new();
    }
}