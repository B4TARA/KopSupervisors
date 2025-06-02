using KOP.Common.Dtos.RecommendationDtos;
using KOP.Common.Enums;

namespace KOP.BLL.Interfaces
{
    public interface IRecommendationService
    {
        Task<List<RecommendationDto>> GetCourseRecommendationsForGrade(int gradeId);
        Task<List<RecommendationDto>> GetLiteratureRecommendationsForGrade(int gradeId);
        Task<List<RecommendationDto>> GetSeminarRecommendationsForGrade(int gradeId);
        Task<List<RecommendationDto>> GetCompetenceRecommendationsForGrade(int gradeId);
        Task ProcessRecommendations(List<RecommendationDto> items, RecommendationTypes type);
    }
}