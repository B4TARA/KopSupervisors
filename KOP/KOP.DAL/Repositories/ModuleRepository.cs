using KOP.DAL.Entities;
using KOP.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KOP.DAL.Repositories
{
    public class ModuleRepository : RepositoryBase<Module>, IModuleRepository
    {
        public ModuleRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<bool> IsNameUniqueAsync(string name)
        {
            return !await _dbContext.Modules.AnyAsync(e => e.Name.ToLower().Replace(" ", "") == name.ToLower().Replace(" ", ""));
        }
    }
}