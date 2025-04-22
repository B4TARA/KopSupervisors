using KOP.DAL.Entities;
using KOP.DAL.Interfaces;

namespace KOP.DAL.Repositories
{
    public class AssessmentMatrixElementRepository : RepositoryBase<AssessmentMatrixElement>, IAssessmentMatrixElementRepository
    {
        public AssessmentMatrixElementRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
