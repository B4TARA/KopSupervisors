using KOP.DAL.Entities.AssessmentEntities;
using KOP.DAL.Interfaces.AssessmentInterfaces;

namespace KOP.DAL.Repositories.AssessmentRepositories
{
    public class AssessmentResultValueRepository : RepositoryBase<AssessmentResultValue>, IAssessmentResultValueRepository
    {
        public AssessmentResultValueRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}