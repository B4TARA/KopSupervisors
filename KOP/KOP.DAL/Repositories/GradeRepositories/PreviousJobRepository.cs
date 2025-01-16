using KOP.DAL.Entities.GradeEntities;
using KOP.DAL.Interfaces.GradeInterfaces;

namespace KOP.DAL.Repositories.GradeRepositories
{
    public class PreviousJobRepository : RepositoryBase<PreviousJob>, IPreviousJobRepository
    {
        public PreviousJobRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}