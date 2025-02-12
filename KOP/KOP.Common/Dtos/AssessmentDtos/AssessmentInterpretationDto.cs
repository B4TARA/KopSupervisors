namespace KOP.Common.Dtos.AssessmentDtos
{
    public class AssessmentInterpretationDto
    {
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
        public string Level { get; set; }
        public string Competence { get; set; }
        public string HtmlClassName { get; set; }
    }
}