using KOP.DAL.Entities.AssessmentEntities;
using KOP.DAL.Interfaces.AssessmentInterfaces;

namespace KOP.DAL.Repositories.AssessmentRepositories
{
    public class AssessmentRangeRepository : RepositoryBase<AssessmentRange>, IAssessmentRangeRepository
    {
        public AssessmentRangeRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}