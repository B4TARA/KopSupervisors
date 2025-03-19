using KOP.Common.Dtos;

namespace KOP.BLL.Interfaces
{
    public interface ISupervisorService
    {
        Task<IEnumerable<SubdivisionDto>> GetSubordinateSubdivisions(int supervisorId);
        Task<IEnumerable<UserSummaryDto>> GetSubordinateUsersSummariesHasGrade(int supervisorId);
        Task ApproveGrade(int gradeId);
    }
}