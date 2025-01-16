using KOP.DAL.Entities;

namespace KOP.DAL.Interfaces
{
    public interface IModuleRepository : IRepositoryBase<Module>
    {
        Task<bool> IsNameUniqueAsync(string name);
    }
}