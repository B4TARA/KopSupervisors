using KOP.Common.Dtos;
using KOP.Common.Dtos.UserDtos;

namespace KOP.BLL.Interfaces
{
    public interface ISupervisorService
    {
        Task<List<SubdivisionDto>> GetSubordinateSubdivisions(int supervisorId);
        Task<List<UserReducedDto>> GetSubordinateUsersSummariesHasGrade(int supervisorId);
        Task ApproveGrade(int gradeId);
    }
}