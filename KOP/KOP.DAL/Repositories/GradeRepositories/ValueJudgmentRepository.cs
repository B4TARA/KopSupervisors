using KOP.DAL.Entities.GradeEntities;
using KOP.DAL.Interfaces.GradeInterfaces;

namespace KOP.DAL.Repositories.GradeRepositories
{
    public class ValueJudgmentRepository : RepositoryBase<ValueJudgment>, IValueJudgmentRepository
    {
        public ValueJudgmentRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}