using KOP.DAL.Entities;

namespace KOP.DAL.Interfaces
{
    public interface IUserRepository : IRepositoryBase<User>
    {
        Task<bool> IsServiceNumberUniqueAsync(int serviceNumber);
    }
}