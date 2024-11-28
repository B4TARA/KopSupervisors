using KOP.DAL.Entities.GradeEntities;
using KOP.DAL.Interfaces.GradeInterfaces;

namespace KOP.DAL.Repositories.GradeRepositories
{
    public class KpiRepository : RepositoryBase<Kpi>, IKpiRepository
    {
        public KpiRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}