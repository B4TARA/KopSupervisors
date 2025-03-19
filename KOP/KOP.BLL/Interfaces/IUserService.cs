using KOP.Common.Dtos;
using KOP.Common.Dtos.AssessmentDtos;
using KOP.Common.Dtos.GradeDtos;
using KOP.Common.Enums;
using KOP.Common.Interfaces;

namespace KOP.BLL.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> GetUser(int id);
        Task<IEnumerable<GradeSummaryDto>> GetUserGradesSummaries(int employeeId);
        Task<List<AssessmentDto>> GetUserLastAssessmentsOfEachAssessmentType(int userId, int supervisorId);
        Task<IBaseResponse<List<AssessmentResultDto>>> GetColleaguesAssessmentResultsForAssessment(int userId);
        Task<IBaseResponse<int>> GetLastAssessmentIdForUserAndType(int userId, SystemAssessmentTypes assessmentType);
        bool CanChooseJudges(IEnumerable<string> userRoles, AssessmentDto assessmentDto);
        Task<IEnumerable<CandidateForJudgeDto>> GetChoosedCandidatesForJudges(IEnumerable<AssessmentResultDto> assessmentResults, int userId);
        Task<IEnumerable<CandidateForJudgeDto>> GetCandidatesForJudges(int userId);

        Task AssessUser(AssessUserDto assessUserDto);
        Task ApproveGrade(int gradeId);
    }
}