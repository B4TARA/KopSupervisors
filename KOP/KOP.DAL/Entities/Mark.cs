namespace KOP.DAL.Entities
{
    public class Mark
    {
        public int Id { get; set; }
        public double PercentageValue { get; set; }
        public string Period { get; set; }

        public MarkType MarkType { get; set; }
        public int MarkTypeId { get; set; }

        public Grade Grade { get; set; }
        public int GradeId { get; set; }

        public DateOnly DateOfCreation { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    }
}