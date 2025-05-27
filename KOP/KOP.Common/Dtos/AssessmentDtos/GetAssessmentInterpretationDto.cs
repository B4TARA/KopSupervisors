namespace KOP.Common.Dtos.AssessmentDtos
{
    public class GetAssessmentInterpretationDto
    {
        public int MinValue { get; init; }
        public int MaxValue { get; init; }
        public string Level { get; init; }
        public string Competence { get; init; }
        public string HtmlClassName { get; init; }
    }
}