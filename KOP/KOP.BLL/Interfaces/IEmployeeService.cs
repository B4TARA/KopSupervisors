using KOP.Common.DTOs;
using KOP.Common.DTOs.AssessmentDTOs;
using KOP.Common.Interfaces;
using KOP.DAL.Entities;

namespace KOP.BLL.Interfaces
{
    public interface IEmployeeService
    {
        Task<IBaseResponse<EmployeeDTO>> GetEmployee(int id);
        Task<IBaseResponse<List<AssessmentDTO>>> GetEmployeeLastAssessments(int employeeId, int supervisorId);
        Task<IBaseResponse<List<AssessmentResultDTO>>> GetColleagueAssessmentResults(int employeeId);
        Task<IBaseResponse<AssessmentResultDTO>> GetSelfAssessment(int employeeId, int assessmentId);
        Task<IBaseResponse<object>> AssessEmployee(AssessEmployeeDTO assessEmployeeDTO);
        Task<IBaseResponse<AssessmentDTO>> GetLastAssessment(int employeeId, int assessmentTypeId);
    }
}