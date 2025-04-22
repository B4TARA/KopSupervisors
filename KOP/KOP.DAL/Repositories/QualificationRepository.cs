using KOP.DAL.Entities;
using KOP.DAL.Interfaces;

namespace KOP.DAL.Repositories
{
    public class QualificationRepository : RepositoryBase<Qualification>, IQualificationRepository
    {
        public QualificationRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}