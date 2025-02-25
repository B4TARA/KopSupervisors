using KOP.Common.Dtos.AssessmentDtos;
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
        Task<IBaseResponse<object>> DeleteJudgeForAssessment(int judgeId, int assessmentId);
        Task<IBaseResponse<object>> AddJudgeForAssessment(int judgeId, int assessmentId, int assignerId);

        // !!! КОСТЫЛЬ-МЕТОД ДЛЯ ВРЕМЕННОЙ ЗАГЛУШКИ !!!
        // Добавить таблицу AssessmentColumns для указания соответствия между столбцом матрицы и диапазоном значений //
        int GetInterpretationColumnByAssessmentValue(int? value);
    }
}