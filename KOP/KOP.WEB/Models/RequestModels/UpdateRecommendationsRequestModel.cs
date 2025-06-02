using KOP.Common.Dtos.RecommendationDtos;

namespace KOP.WEB.Models.RequestModels
{
    public class UpdateRecommendationsRequestModel
    {
        public List<RecommendationDto> Competences { get; set; }
        public List<RecommendationDto> Literature { get; set; }
        public List<RecommendationDto> Courses { get; set; }
        public List<RecommendationDto> Seminars { get; set; }
    }
}