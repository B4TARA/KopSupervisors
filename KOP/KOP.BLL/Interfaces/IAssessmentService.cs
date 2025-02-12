using KOP.Common.Dtos.AssessmentDtos;
using KOP.Common.Enums;
using KOP.Common.Interfaces;

namespace KOP.BLL.Interfaces
{
    public interface IAssessmentService
    {
        Task<IBaseResponse<AssessmentDto>> GetAssessment(int id);
        Task<IBaseResponse<AssessmentResultDto>> GetAssessmentResult(int judgeId, int assessmentId);
        Task<IBaseResponse<AssessmentTypeDto>> GetAssessmentType(int userId, int assessmentTypeId);
        Task<IBaseResponse<AssessmentSummaryDto>> GetAssessmentSummary(int assessmentId);
        Task<IBaseResponse<bool>> IsActiveAssessment(int judgeId, int judgedId, int? assessmentId = null);
    }
}