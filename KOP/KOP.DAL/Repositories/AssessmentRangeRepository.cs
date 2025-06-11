using KOP.DAL.Entities;
using KOP.DAL.Interfaces;

namespace KOP.DAL.Repositories
{
    public class AssessmentRangeRepository : RepositoryBase<AssessmentRange>, IAssessmentRangeRepository
    {
        public AssessmentRangeRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}