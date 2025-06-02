using KOP.Common.Dtos.RecommendationDtos;

namespace KOP.Common.Dtos.AnalyticsDtos
{
    public class CompetenciesAnalyticsDto
    {
        public List<CompetenceDto> TopCompetencies { get; set; }
        public List<CompetenceDto> AntiTopCompetencies { get; set; }
        public List<RecommendationDto> CompetenceRecommendations { get; set; }
        public List<RecommendationDto> LiteratureRecommendations { get; set; }
        public List<RecommendationDto> CourseRecommendations { get; set; }
        public List<RecommendationDto> SeminarRecommendations { get; set; }
    }

    public class CompetenceDto
    {
        public double avgValue { get; set; }
        public string Name { get; set; }
        public string CompetenceDescription { get; set; }
    }
}