using KOP.DAL.Entities;
using KOP.DAL.Interfaces;

namespace KOP.DAL.Repositories
{
    public class AssessmentInterpretationRepository : RepositoryBase<AssessmentInterpretation>, IAssessmentInterpretationRepository
    {
        public AssessmentInterpretationRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}