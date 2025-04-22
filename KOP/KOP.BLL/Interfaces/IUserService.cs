using KOP.Common.Dtos;
using KOP.Common.Dtos.AssessmentDtos;
using KOP.Common.Dtos.GradeDtos;

namespace KOP.BLL.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> GetUserDto(int userId);
        Task<List<AssessmentDto>> GetUserLastGradeAssessmentDtoList(int userId);
        Task<List<GradeReducedDto>> GetUserGradeSummaryDtoList(int userId);
        Task<List<AssessmentResultDto>> GetColleaguesAssessmentResultsForAssessment(int userId);

        Task AssessUser(AssessUserDto assessUserDto);
        Task ApproveGrade(int gradeId);

        bool CanChooseJudges(List<string> userRoles, AssessmentDto assessmentDto);
    }
}