namespace KOP.DAL.Entities.AssessmentEntities
{
    public class AssessmentMatrixElement
    {
        public int Id { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public string Value { get; set; }
        public string HtmlClassName { get; set; }

        public int MatrixId { get; set; }
        public AssessmentMatrix Matrix { get; set; }

        public DateOnly DateOfCreation { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    }
}