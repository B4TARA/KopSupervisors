using KOP.Common.Dtos;
using KOP.Common.Dtos.AssessmentDtos;
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
        Task<IBaseResponse<AssessmentDto>> GetLastAssessmentByAssessmentType(int userId, int assessmentTypeId);
    }
}