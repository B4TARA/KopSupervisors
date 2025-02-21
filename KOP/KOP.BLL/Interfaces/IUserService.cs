using KOP.Common.Dtos;
using KOP.Common.Dtos.AssessmentDtos;
using KOP.Common.Enums;
using KOP.Common.Interfaces;

namespace KOP.BLL.Interfaces
{
    public interface IUserService
    {
        Task<IBaseResponse<UserDto>> GetUser(int id);
        Task<IBaseResponse<List<AssessmentDto>>> GetUserLastAssessmentsOfEachAssessmentType(int userId, int supervisorId);
        Task<IBaseResponse<List<AssessmentResultDto>>> GetColleaguesAssessmentResultsForAssessment(int userId);
        Task<IBaseResponse<AssessmentResultDto>> GetUserSelfAssessmentResultByAssessment(int userId, int assessmentId);
        Task<IBaseResponse<object>> AssessUser(AssessUserDto assessUserDto);
        Task<IBaseResponse<int>> GetLastAssessmentIdForUserAndType(int userId, SystemAssessmentTypes assessmentType);
        Task<IBaseResponse<List<AssessmentTypeDto>>> GetAssessmentTypesForUser(int userId);
        bool CanChooseJudges(IEnumerable<string> userRoles, AssessmentDto assessmentDto);
        Task<List<CandidateForJudgeDto>> GetChoosedCandidatesForJudges(List<AssessmentResultDto> assessmentResults, int userId);
        Task<List<CandidateForJudgeDto>> GetCandidatesForJudges(int userId);
        Task<IBaseResponse<object>> ApproveGrade(int gradeId);
    }
}