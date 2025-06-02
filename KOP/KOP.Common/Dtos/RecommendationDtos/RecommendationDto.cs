namespace KOP.Common.Dtos.RecommendationDtos
{
    public class RecommendationDto
    {
        public int Id { get; set; }
        public int GradeId { get; set; }
        public string Value { get; set; }
        public bool IsDeleted { get; set; }
    }
}