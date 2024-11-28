using KOP.DAL.Entities.GradeEntities;
using KOP.DAL.Interfaces.GradeInterfaces;

namespace KOP.DAL.Repositories.GradeRepositories
{
    public class MarkTypeRepository : RepositoryBase<MarkType>, IMarkTypeRepository
    {
        public MarkTypeRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
