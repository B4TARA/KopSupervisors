using KOP.DAL.Entities;
using KOP.DAL.Interfaces;

namespace KOP.DAL.Repositories
{
    public class AssessmentRepository : RepositoryBase<Assessment>, IAssessmentRepository
    {
        public AssessmentRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
