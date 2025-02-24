using KOP.Common.Dtos;
using KOP.Common.Interfaces;
using KOP.DAL.Entities;

namespace KOP.BLL.Interfaces
{
    public interface ISupervisorService
    {
        Task<User?> GetSupervisorForUser(int userId);
        Task<IBaseResponse<IEnumerable<SubdivisionDto>>> GetUserSubordinateSubdivisions(int supervisorId);
        Task<IBaseResponse<List<UserDto>>> GetSubordinateUsers(int supervisorId);
        Task<IBaseResponse<object>> ApproveEmployeeGrade(int gradeId);
    }
}