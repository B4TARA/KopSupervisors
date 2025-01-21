using KOP.DAL.Entities;

namespace KOP.DAL.Interfaces
{
    public interface ISubdivisionRepository : IRepositoryBase<Subdivision>
    {
        Task<bool> IsNameUniqueAsync(string name);
    }
}