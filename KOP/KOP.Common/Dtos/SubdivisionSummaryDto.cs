namespace KOP.Common.Dtos
{
    public class SubdivisionSummaryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public bool IsSelected { get; set; }
    }
}