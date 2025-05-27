using KOP.Common.Dtos;

namespace KOP.BLL.Interfaces
{
    public interface ISupervisorService
    {
        Task<List<SubdivisionDto>> GetSubordinateSubdivisions(int supervisorId);
        Task<List<UserSummaryDto>> GetSubordinateUsersSummariesHasGrade(int supervisorId);
        Task ApproveGrade(int gradeId);
    }
}