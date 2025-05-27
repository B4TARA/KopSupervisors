using KOP.Common.Dtos;
using KOP.Common.Dtos.AssessmentDtos;

namespace KOP.WEB.Models.ViewModels.Admin
{
    public class UserRecommendationsViewModel
    {
        public int SelfAssessmentSum { get; set; }
        public int SupervisorAssessmentSum { get; set; }
        public GetAssessmentInterpretationDto AssessmentInterpretation { get; set; } = new();
        public List<GetRecommendationDto> CompetenceRecommendationList { get; set; } = new();
        public List<GetRecommendationDto> LiteratureRecommendationList { get; set; } = new();
        public List<GetRecommendationDto> CourseRecommendationList { get; set; } = new();
        public List<GetRecommendationDto> SeminarRecommendationList { get; set; } = new();
    }
}