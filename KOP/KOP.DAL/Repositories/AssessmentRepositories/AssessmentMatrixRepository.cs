using KOP.DAL.Entities.AssessmentEntities;
using KOP.DAL.Interfaces.AssessmentInterfaces;

namespace KOP.DAL.Repositories.AssessmentRepositories
{
    public class AssessmentMatrixRepository : RepositoryBase<AssessmentMatrix>, IAssessmentMatrixRepository
    {
        public AssessmentMatrixRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
