using KOP.DAL.Entities;
using KOP.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KOP.DAL.Repositories
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<bool> IsServiceNumberUniqueAsync(int serviceNumber)
        {
            return !await _dbContext.Users.AnyAsync(e => e.ServiceNumber == serviceNumber);
        }
    }
}