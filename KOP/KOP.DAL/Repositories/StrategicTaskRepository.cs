using KOP.DAL.Entities;
using KOP.DAL.Interfaces;

namespace KOP.DAL.Repositories
{
    public class StrategicTaskRepository : RepositoryBase<StrategicTask>, IStrategicTaskRepository
    {
        public StrategicTaskRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}