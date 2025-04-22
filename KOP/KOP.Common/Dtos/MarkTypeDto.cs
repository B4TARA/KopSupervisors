namespace KOP.Common.Dtos.GradeDtos
{
    public class MarkTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<MarkDto> Marks { get; set; } = new();
    }
}