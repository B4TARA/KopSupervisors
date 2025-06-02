using KOP.Common.Dtos.AssessmentDtos;
using KOP.Common.Dtos.RecommendationDtos;

namespace KOP.WEB.Models.ViewModels.Admin
{
    public class UserRecommendationsViewModel
    {
        public int UserId { get; set; }
        public int GradeId { get; set; }
        public int SelfAssessmentSum { get; set; }
        public int SupervisorAssessmentSum { get; set; }
        public AssessmentInterpretationDto AssessmentInterpretation { get; set; }
        public List<RecommendationDto> CompetenceRecommendations { get; set; }
        public List<RecommendationDto> LiteratureRecommendations { get; set; }
        public List<RecommendationDto> CourseRecommendations { get; set; }
        public List<RecommendationDto> SeminarRecommendations { get; set; }
    }
}