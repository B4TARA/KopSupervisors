using KOP.DAL.Entities;
using KOP.DAL.Interfaces;

namespace KOP.DAL.Repositories
{
    public class ValueJudgmentRepository : RepositoryBase<ValueJudgment>, IValueJudgmentRepository
    {
        public ValueJudgmentRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}