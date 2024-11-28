using KOP.DAL.Entities;
using KOP.DAL.Interfaces;

namespace KOP.DAL.Repositories
{
    public class ModuleTypeRepository : RepositoryBase<ModuleType>, IModuleTypeRepository
    {
        public ModuleTypeRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}