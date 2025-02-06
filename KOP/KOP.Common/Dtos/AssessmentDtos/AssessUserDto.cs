namespace KOP.Common.Dtos.AssessmentDtos
{
    public class AssessUserDto
    {
        public int AssessmentResultId { get; set; }
        public List<string> ResultValues { get; set; } = new();
    }
}