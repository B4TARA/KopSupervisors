using KOP.DAL.Entities;
using KOP.DAL.Interfaces;

namespace KOP.DAL.Repositories
{
    public class MarkTypeRepository : RepositoryBase<MarkType>, IMarkTypeRepository
    {
        public MarkTypeRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
