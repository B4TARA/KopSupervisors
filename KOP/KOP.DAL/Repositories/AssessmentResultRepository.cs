using KOP.DAL.Entities;
using KOP.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KOP.DAL.Repositories
{
    public class AssessmentResultRepository : RepositoryBase<AssessmentResult>, IAssessmentResultRepository
    {
        public AssessmentResultRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<bool> IsServiceNumberUniqueAsync(int serviceNumber)
        {
            return !await _dbContext.Users.AnyAsync(e => e.ServiceNumber == serviceNumber);
        }
    }
}