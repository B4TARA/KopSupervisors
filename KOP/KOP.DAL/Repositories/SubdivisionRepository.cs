using KOP.DAL.Entities;
using KOP.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KOP.DAL.Repositories
{
    public class SubdivisionRepository : RepositoryBase<Subdivision>, ISubdivisionRepository
    {
        public SubdivisionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<bool> IsNameUniqueAsync(string name)
        {
            return !await _dbContext.Subdivisions.AnyAsync(e => e.Name.ToLower().Replace(" ", "") == name.ToLower().Replace(" ", ""));
        }
    }
}