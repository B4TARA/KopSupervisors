using KOP.DAL.Entities.GradeEntities;
using KOP.DAL.Interfaces.GradeInterfaces;

namespace KOP.DAL.Repositories.GradeRepositories
{
    public class TrainingEventRepository : RepositoryBase<TrainingEvent>, ITrainingEventRepository
    {
        public TrainingEventRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}