using KOP.Common.Dtos;
using KOP.Common.Interfaces;

namespace KOP.BLL.Interfaces
{
    public interface ISupervisorService
    {
        Task<IBaseResponse<IEnumerable<SubdivisionDto>>> GetUserSubordinateSubdivisions(int supervisorId, bool onlySubdivisionsWithPendingUsersGrades);
        Task<IBaseResponse<List<UserDto>>> GetSubordinateUsers(int supervisorId);
    }
}