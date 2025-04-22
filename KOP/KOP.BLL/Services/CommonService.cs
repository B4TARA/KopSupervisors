using KOP.BLL.Interfaces;
using KOP.Common.Enums;
using KOP.DAL;
using KOP.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace KOP.BLL.Services
{
    public class CommonService : ICommonService
    {
        private readonly ApplicationDbContext _dbContext;

        public CommonService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User?> GetFirstSupervisorForUser(int userId)
        {
            // Проверка на существование пользователя
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                throw new Exception($"User with ID {userId} not found.");
            }

            // Получение родительского подразделения
            var parentSubdivision = await _dbContext.Subdivisions
                .Include(s => s.Parent)
                .FirstOrDefaultAsync(s => s.Id == user.ParentSubdivisionId);

            if (parentSubdivision == null)
            {
                return null;
            }

            // Получение руководителя
            var supervisor = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.SystemRoles.Contains(SystemRoles.Supervisor) && u.SubordinateSubdivisions.Contains(parentSubdivision));

            if (supervisor != null)
            {
                return supervisor;
            }

            // Поиск руководителя в родительских подразделениях
            var rootSubdivision = parentSubdivision.Parent;

            while (rootSubdivision != null)
            {
                supervisor = await _dbContext.Users
                    .FirstOrDefaultAsync(u => u.SystemRoles.Contains(SystemRoles.Supervisor) && u.SubordinateSubdivisions.Contains(rootSubdivision));

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