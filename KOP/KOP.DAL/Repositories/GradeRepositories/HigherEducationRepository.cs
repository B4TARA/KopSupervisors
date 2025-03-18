using KOP.DAL.Entities.GradeEntities;
using KOP.DAL.Interfaces.GradeInterfaces;

namespace KOP.DAL.Repositories.GradeRepositories
{
    public class HigherEducationRepository : RepositoryBase<HigherEducation>, IHigherEducationRepository
    {
        public HigherEducationRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}