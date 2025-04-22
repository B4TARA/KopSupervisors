using KOP.DAL.Entities;
using KOP.DAL.Interfaces;

namespace KOP.DAL.Repositories
{
    public class AssessmentMatrixRepository : RepositoryBase<AssessmentMatrix>, IAssessmentMatrixRepository
    {
        public AssessmentMatrixRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
