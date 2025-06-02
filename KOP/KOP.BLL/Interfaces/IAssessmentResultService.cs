using KOP.Common.Dtos.AssessmentResultDtos;
using KOP.DAL.Entities;

namespace KOP.BLL.Interfaces
{
    public interface IAssessmentResultService
    {
        Task<AssessmentResultDto?> GetAssessmentResultDto(int judgeId, int assessmentId);
        Task CreatePendingColleagueAssessmentResult(int judgeId, int assessmentId, int assignerId);
        Task DeletePendingColleagueAssessmentResult(int id);
        Task CreateAssessmentResult(AssessmentResult assessmentResult);
        Task DeleteAssessmentResult(AssessmentResult assessmentResult);

        Task<AssessmentResultDto?> GetManagementSelfAssessmentResultForGrade(int gradeId);
        Task<AssessmentResultDto?> GetManagementSupervisorAssessmentResultForGrade(int gradeId);
    }
}