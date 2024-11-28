using KOP.DAL.Entities.GradeEntities;
using KOP.DAL.Interfaces.GradeInterfaces;

namespace KOP.DAL.Repositories.GradeRepositories
{
    public class StrategicTaskRepository : RepositoryBase<StrategicTask>, IStrategicTaskRepository
    {
        public StrategicTaskRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}