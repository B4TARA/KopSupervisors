using KOP.DAL.Entities;
using KOP.DAL.Interfaces;

namespace KOP.DAL.Repositories
{
    public class TrainingEventRepository : RepositoryBase<TrainingEvent>, ITrainingEventRepository
    {
        public TrainingEventRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}