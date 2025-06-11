using KOP.DAL.Entities;
using KOP.DAL.Interfaces;

namespace KOP.DAL.Repositories
{
    public class AssessmentTypeRepository : RepositoryBase<AssessmentType>, IAssessmentTypeRepository
    {
        public AssessmentTypeRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
