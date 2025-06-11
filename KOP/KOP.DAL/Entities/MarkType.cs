namespace KOP.DAL.Entities
{
    public class MarkType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public List<Mark> Marks { get; set; } = new();

        public DateOnly DateOfCreation { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    }
}