namespace KOP.DAL.Entities.AssessmentEntities
{
    public class AssessmentRange
    {
        public int Id { get; set; }
        public int MinRangeValue { get; set; }
        public int MaxRangeValue { get; set; }
        public int ColumnNumber { get; set; }
    }
}