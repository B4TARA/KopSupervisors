using KOP.DAL.Entities.AssessmentEntities;
using KOP.DAL.Interfaces.AssessmentInterfaces;

namespace KOP.DAL.Repositories.AssessmentRepositories
{
    public class AssessmentInterpretationRepository : RepositoryBase<AssessmentInterpretation>, IAssessmentInterpretationRepository
    {
        public AssessmentInterpretationRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}