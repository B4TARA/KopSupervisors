namespace KOP.Common.Dtos
{
    public class CandidateForJudgeDto
    {
        public int Id { get; set; }
        public int AssessmentResultId { get; set; }
        public string FullName { get; set; }
        public bool HasJudged { get; set; }
    }
}