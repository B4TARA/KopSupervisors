namespace KOP.DAL.Entities.AssessmentEntities
{
    public class AssessmentInterpretation
    {
        public int Id { get; set; }
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
        public string Level { get; set; }
        public string Competence { get; set; }
        public string HtmlClassName { get; set; }

        public int AssessmentTypeId { get; set; }
        public AssessmentType AssessmentType { get; set; }

        public DateOnly DateOfCreation { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    }
}