using KOP.DAL.Entities;
using KOP.DAL.Interfaces;

namespace KOP.DAL.Repositories
{
    public class AssessmentResultValueRepository : RepositoryBase<AssessmentResultValue>, IAssessmentResultValueRepository
    {
        public AssessmentResultValueRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}