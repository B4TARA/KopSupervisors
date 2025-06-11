using KOP.Common.Dtos.AssessmentResultDtos;
using KOP.DAL.Entities;

namespace KOP.BLL.Interfaces
{
    public interface IAssessmentResultService
    {
        Task<AssessmentResultDto?> GetAssessmentResult(int judgeId, int assessmentId);
        Task<AssessmentResultDto?> GetManagementSelfAssessmentResultForGrade(int gradeId);
        Task<AssessmentResultDto?> GetManagementSupervisorAssessmentResultForGrade(int gradeId);

        Task CreateAssessmentResult(AssessmentResult assessmentResult);
        Task CreatePendingColleagueAssessmentResult(int judgeId, int assessmentId, int assignerId);
        Task DeletePendingAssessmentResult(int id);

    }
}