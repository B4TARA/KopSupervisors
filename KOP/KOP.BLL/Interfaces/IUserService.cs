using KOP.Common.Dtos.AssessmentDtos;
using KOP.Common.Dtos.AssessmentResultDtos;
using KOP.Common.Dtos.GradeDtos;
using KOP.Common.Dtos.UserDtos;

namespace KOP.BLL.Interfaces
{
    public interface IUserService
    {
        Task<UserExtendedDto> GetUserDto(int userId);
        Task<List<AssessmentDto>> GetUserLastGradeAssessmentDtoList(int userId);
        Task<List<GradeReducedDto>> GetUserGradeSummaryDtoList(int userId);
        Task<List<AssessmentResultDto>> GetColleaguesAssessmentResultsForAssessment(int userId);

        Task AssessUser(AssessUserDto assessUserDto);
        Task ApproveGrade(int gradeId);

        bool CanChooseJudges(List<string> userRoles, AssessmentDto assessmentDto);

        Task<List<UserReducedDto>> GetAllUsers();
        Task<List<UserReducedDto>> GetUsersWithAnyPendingGrade();
    }
}