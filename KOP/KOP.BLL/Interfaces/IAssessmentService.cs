using KOP.Common.Dtos.AssessmentDtos;

namespace KOP.BLL.Interfaces
{
    public interface IAssessmentService
    {
        Task<AssessmentDto> GetAssessment(int id);
        Task<AssessmentResultDto?> GetAssessmentResult(int judgeId, int assessmentId);
        Task<AssessmentSummaryDto> GetAssessmentSummary(int assessmentId);
        Task<bool> IsActiveAssessment(int judgeId, int judgedId, int? assessmentId = null);
        Task DeleteJudgeForAssessment(int assessmentResultId);
        Task AddJudgeForAssessment(int judgeId, int assessmentId, int assignerId);

        // !!! КОСТЫЛЬ-МЕТОД ДЛЯ ВРЕМЕННОЙ ЗАГЛУШКИ !!!
        // Добавить таблицу AssessmentColumns для указания соответствия между столбцом матрицы и диапазоном значений //
        int GetInterpretationColumnByAssessmentValue(int? value);
    }
}