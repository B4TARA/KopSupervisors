using KOP.Common.DTOs;
using KOP.Common.Interfaces;

namespace KOP.BLL.Interfaces
{
    public interface ISupervisorService
    {
        Task<IBaseResponse<List<ModuleDTO>>> GetUserSubordinateSubdivisions(int supervisorId);
    }
}