namespace KOP.DAL.Entities.AssessmentEntities
{
    public class AssessmentResultValue
    {
        public int Id { get; set; }
        public int Value { get; set; }
        public int AssessmentMatrixRow { get; set; }

        public AssessmentResult AssessmentResult { get; set; }
        public int AssessmentResultId { get; set; }

        public DateOnly DateOfCreation { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    }
}