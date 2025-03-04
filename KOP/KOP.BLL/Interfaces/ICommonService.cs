using KOP.DAL.Entities;

namespace KOP.BLL.Interfaces
{
    public interface ICommonService
    {
        Task<User?> GetSupervisorForUser(int userId);
    }
}