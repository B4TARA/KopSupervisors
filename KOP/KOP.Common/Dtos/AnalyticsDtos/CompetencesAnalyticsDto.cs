namespace KOP.Common.Dtos.AnalyticsDtos
{
    public class CompetenciesAnalyticsDto
    {
        public List<CompetenceDto> TopCompetencies { get; set; } = new();
        public List<CompetenceDto> AntiTopCompetencies { get; set; } = new();
    }

    public class CompetenceDto
    {
        public double avgValue { get; set; }
        public string Name { get; set; }
        public string CompetenceDescription { get; set; }
    }
}