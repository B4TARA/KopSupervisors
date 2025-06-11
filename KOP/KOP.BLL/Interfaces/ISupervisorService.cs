using KOP.Common.Dtos;
using KOP.Common.Dtos.UserDtos;

namespace KOP.BLL.Interfaces
{
    public interface ISupervisorService
    {
        Task<List<SubdivisionDto>> GetSubdivisionsForSupervisor(int supervisorId);
        Task<List<UserDto>> GetUsersWithAnyGradeForSupervisor(int supervisorId);
        Task<List<UserDto>> GetUsersWithAnyUpcomingGradeForSupervisor(int supervisorId);
        Task ApproveGrade(int gradeId);
        Task DismissUser(int userId);
    }
}