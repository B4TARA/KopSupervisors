using KOP.DAL.Entities;
using KOP.DAL.Interfaces;

namespace KOP.DAL.Repositories
{
    public class PreviousJobRepository : RepositoryBase<PreviousJob>, IPreviousJobRepository
    {
        public PreviousJobRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}