using KOP.DAL.Entities.AssessmentEntities;
using KOP.DAL.Interfaces.AssessmentInterfaces;

namespace KOP.DAL.Repositories.AssessmentRepositories
{
    public class AssessmentMatrixElementRepository : RepositoryBase<AssessmentMatrixElement>, IAssessmentMatrixElementRepository
    {
        public AssessmentMatrixElementRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
