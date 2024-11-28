using KOP.Common.DTOs.AssessmentDTOs;
using KOP.Common.Enums;
using KOP.Common.Interfaces;

namespace KOP.BLL.Interfaces
{
    public interface IAssessmentService
    {
        Task<IBaseResponse<AssessmentDTO>> GetAssessment(int id, SystemStatuses? systemStatus = null);
        Task<IBaseResponse<AssessmentResultDTO>> GetAssessmentResult(int judgeId, int assessmentId);
        Task<IBaseResponse<AssessmentTypeDTO>> GetAssessmentType(int employeeId, int assessmentTypeId);
        Task<IBaseResponse<List<AssessmentTypeDTO>>> GetAssessmentTypes(int employeeId);
        Task<IBaseResponse<bool>> IsActiveAssessment(int judgeId, int judgedId);
        Task<IBaseResponse<bool>> IsActiveAssessment(int judgeId, int judgedId, int assessmentId);
    }
}