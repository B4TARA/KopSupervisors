using KOP.DAL.Entities.AssessmentEntities;
using KOP.DAL.Interfaces.AssessmentInterfaces;

namespace KOP.DAL.Repositories.AssessmentRepositories
{
    public class AssessmentTypeRepository : RepositoryBase<AssessmentType>, IAssessmentTypeRepository
    {
        public AssessmentTypeRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
