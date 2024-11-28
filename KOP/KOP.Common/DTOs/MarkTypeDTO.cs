namespace KOP.Common.DTOs
{
    public class MarkTypeDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<MarkDTO> Marks { get; set; } = new();
    }
}