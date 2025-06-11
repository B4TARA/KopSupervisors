using KOP.DAL.Entities;
using KOP.DAL.Interfaces;

namespace KOP.DAL.Repositories
{
    public class KpiRepository : RepositoryBase<Kpi>, IKpiRepository
    {
        public KpiRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}