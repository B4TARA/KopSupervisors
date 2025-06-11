using KOP.Common.Dtos.AssessmentDtos;
using KOP.Common.Enums;
using KOP.DAL.Entities;

namespace KOP.BLL.Interfaces
{
    public interface IAssessmentService
    {
        Task<AssessmentDto> GetAssessment(int id);
        Task<AssessmentInterpretationDto?> GetCorporateAssessmentInterpretationForGrade(int gradeId);
        Task<AssessmentInterpretationDto?> GetManagementAssessmentInterpretationForGrade(int gradeId);
        Task<AssessmentSummaryDto> GetAssessmentSummary(int assessmentId);
        Task<bool> IsActiveAssessment(int judgeId, int judgedId, int? assessmentId = null);
        Task<int> GetMatrixColumnForAssessmentValue(int value);
        bool IsAssessmentFinalized(SystemAssessmentTypes? assessmentType, List<AssessmentResult> completedResults);
    }
}