using KOP.Common.DTOs;
using KOP.Common.Interfaces;

namespace KOP.BLL.Interfaces
{
    public interface ISupervisorService
    {
        Task<IBaseResponse<ModuleDTO>> GetAllSubordinateModules(int supervisorId);
    }
}