using KOP.Common.DTOs;
using KOP.Common.DTOs.AssessmentDTOs;
using KOP.Common.Interfaces;

namespace KOP.BLL.Interfaces
{
    public interface IUserService
    {
        Task<IBaseResponse<EmployeeDTO>> GetUser(int id);
        Task<IBaseResponse<List<AssessmentDTO>>> GetUserLastAssessmentsOfEachAssessmentType(int userId, int supervisorId);
        Task<IBaseResponse<List<AssessmentResultDTO>>> GetColleaguesAssessmentResultsForAssessment(int userId);
        Task<IBaseResponse<AssessmentResultDTO>> GetUserSelfAssessmentResultByAssessment(int userId, int assessmentId);
        Task<IBaseResponse<object>> AssessUser(AssessEmployeeDTO assessUserDto);
        Task<IBaseResponse<AssessmentDTO>> GetLastAssessmentByAssessmentType(int userId, int assessmentTypeId);
    }
}