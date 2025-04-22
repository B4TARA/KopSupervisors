using KOP.DAL.Entities;
using KOP.DAL.Interfaces;

namespace KOP.DAL.Repositories
{
    public class HigherEducationRepository : RepositoryBase<HigherEducation>, IHigherEducationRepository
    {
        public HigherEducationRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}