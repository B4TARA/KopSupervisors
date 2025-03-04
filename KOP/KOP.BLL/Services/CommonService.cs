using KOP.BLL.Interfaces;
using KOP.Common.Enums;
using KOP.DAL.Entities;
using KOP.DAL.Interfaces;

namespace KOP.BLL.Services
{
    public class CommonService : ICommonService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CommonService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<User?> GetSupervisorForUser(int userId)
        {
            var user = await _unitOfWork.Users.GetAsync(x => x.Id == userId);
            var parentSubdivision = await _unitOfWork.Subdivisions.GetAsync(x => x.Id == user.ParentSubdivisionId, includeProperties: "Parent");

            if (parentSubdivision == null)
            {
                return null;
            }

            var supervisor = await _unitOfWork.Users.GetAsync(x => x.SystemRoles.Contains(SystemRoles.Supervisor) && x.SubordinateSubdivisions.Contains(parentSubdivision));

            if (supervisor != null)
            {
                return supervisor;
            }

            var rootSubdivision = parentSubdivision.Parent;

            while (rootSubdivision != null)
            {
                supervisor = await _unitOfWork.Users.GetAsync(x => x.SystemRoles.Contains(SystemRoles.Supervisor) && x.SubordinateSubdivisions.Contains(rootSubdivision));

                if (supervisor != null)
                {
                    return supervisor;
                }

                rootSubdivision = rootSubdivision.Parent;
            }

            return null;
        }
    }
}