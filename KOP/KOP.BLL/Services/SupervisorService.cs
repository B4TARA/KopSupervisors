using KOP.BLL.Interfaces;
using KOP.Common.Dtos;
using KOP.Common.Dtos.GradeDtos;
using KOP.Common.Dtos.UserDtos;
using KOP.Common.Enums;
using KOP.DAL;
using KOP.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace KOP.BLL.Services
{
    public class SupervisorService : ISupervisorService
    {
        private readonly ApplicationDbContext _context;
        private readonly IGradeService _gradeService;

        public SupervisorService(ApplicationDbContext context, IGradeService gradeService)
        {
            _context = context;
            _gradeService = gradeService;
        }

        public async Task<List<SubdivisionDto>> GetSubdivisionsForSupervisor(int supervisorId)
        {
            var supervisor = await _context.Users
                .AsNoTracking()
                .Where(u => u.Id == supervisorId)
                .Include(u => u.SubordinateSubdivisions)
                    .ThenInclude(s => s.Users
                        .Where(u => !u.IsDismissed))
                    .ThenInclude(u => u.Grades)
                .Include(u => u.SubordinateSubdivisions)
                    .ThenInclude(s => s.Children)
                    .ThenInclude(c => c.Users
                        .Where(u => !u.IsDismissed))
                    .ThenInclude(u => u.Grades)
                .FirstOrDefaultAsync();

            if (supervisor == null)
                throw new Exception($"Supervisor with ID {supervisorId} not found.");

            var subdivisions = new List<SubdivisionDto>();

            foreach (var subdivision in supervisor.SubordinateSubdivisions)
            {
                var subdivisionDto = await ProcessSubdivision(subdivision);

                if (subdivisionDto != null)
                    subdivisions.Add(subdivisionDto);
            }

            return subdivisions;
        }

        private async Task<SubdivisionDto?> ProcessSubdivision(Subdivision subdivision)
        {
            var subdivisionDto = new SubdivisionDto
            {
                Id = subdivision.Id,
                Name = subdivision.Name,
                IsRoot = subdivision.NestingLevel == 1
            };

            subdivisionDto.Users = subdivision.Users
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Position = u.Position,
                    NextGradeStartDate = u.GetNextGradeStartDate.Value,
                    LastGrade = u.Grades
                        .OrderByDescending(g => g.Number)
                        .Select(g => new GradeDto
                        {
                            IsPending = g.GradeStatus == GradeStatuses.PENDING,
                            CompletedСriteriaCount = _gradeService.CalculateCompletedCriteriaCount(g),
                        })
                        .FirstOrDefault(),
                })
                .ToList();

            foreach (var child in subdivision.Children)
            {
                var childSubdivisionDto = await ProcessSubdivision(child);

                if (childSubdivisionDto != null)
                    subdivisionDto.Children.Add(childSubdivisionDto);
            }

            return subdivisionDto.Users.Any() || subdivisionDto.Children.Any() ? subdivisionDto : null;
        }

        private async Task<List<UserDto>> GetUsersForSupervisor(int supervisorId, Func<User, bool> gradeFilter)
        {
            var supervisor = await _context.Users
                .AsNoTracking()
                .Where(u => u.Id == supervisorId)
                .Include(u => u.SubordinateSubdivisions)
                    .ThenInclude(s => s.Users)
                    .ThenInclude(u => u.Grades)
                .Include(u => u.SubordinateSubdivisions)
                    .ThenInclude(s => s.Children)
                    .ThenInclude(c => c.Users)
                    .ThenInclude(u => u.Grades)
                .FirstOrDefaultAsync();

            if (supervisor == null)
                throw new Exception($"User with ID {supervisorId} not found.");

            var allSubordinateUsers = new List<UserDto>();

            foreach (var subdivision in supervisor.SubordinateSubdivisions)
            {
                var subordinateUsers = await GetUsersForSubdivision(subdivision, gradeFilter);
                allSubordinateUsers.AddRange(subordinateUsers);
            }

            return allSubordinateUsers;
        }

        private async Task<List<UserDto>> GetUsersForSubdivision(Subdivision subdivision, Func<User, bool> gradeFilter)
        {
            var subordinateUsers = new List<UserDto>();
            var filteredUsers = subdivision.Users
                .Where(u => u.SystemRoles.Contains(SystemRoles.Employee) && gradeFilter(u))
                .OrderBy(u => u.FullName)
                .ToList();

            foreach (var user in filteredUsers)
            {
                subordinateUsers.Add(new UserDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Position = user.Position,
                    SubdivisionFromFile = user.SubdivisionFromFile,
                    NextGradeStartDate = user.GetNextGradeStartDate.Value,
                    ContractEndDate = user.GetContractEndDate,
                });
            }

            foreach (var childSubdivision in subdivision.Children)
            {
                var subordinateUsersFromChild = await GetUsersForSubdivision(childSubdivision, gradeFilter);
                subordinateUsers.AddRange(subordinateUsersFromChild);
            }

            return subordinateUsers;
        }

        public async Task<List<UserDto>> GetUsersWithAnyGradeForSupervisor(int supervisorId)
        {
            var users = await GetUsersForSupervisor(supervisorId, u => u.Grades.Any());
            var orderedUser = users
                .OrderBy(u => u.FullName)
                .ToList();

            return orderedUser;
        } 

        public async Task<List<UserDto>> GetUsersWithAnyUpcomingGradeForSupervisor(int supervisorId)
        {
            var users = await GetUsersForSupervisor(supervisorId, u => u.GetNextGradeStartDate.HasDateValue && !u.IsDismissed);
            var orderedUser = users
              .OrderBy(u => u.FullName)
              .ToList();

            return orderedUser;
        }

        public async Task ApproveGrade(int gradeId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var grade = await _context.Grades.FindAsync(gradeId);
                if (grade == null)
                    throw new Exception($"Grade with ID {gradeId} not found.");

                grade.GradeStatus = GradeStatuses.COMPLETED;
                grade.SystemStatus = SystemStatuses.COMPLETED;

                await _context.AssessmentResults
                    .Where(ar => ar.Assessment.GradeId == gradeId && ar.SystemStatus == SystemStatuses.PENDING)
                    .ExecuteDeleteAsync();

                await _context.Assessments
                    .Where(a => a.GradeId == gradeId)
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(a => a.SystemStatus, SystemStatuses.COMPLETED));

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task DismissUser(int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                    throw new Exception($"User with ID {userId} not found.");

                user.IsDismissed = true;

                var latestGrade = await _context.Grades
                    .Where(g => g.UserId == userId && g.SystemStatus == SystemStatuses.PENDING)
                    .OrderByDescending(g => g.Number)
                    .FirstOrDefaultAsync();

                if (latestGrade != null)
                {
                    latestGrade.EndDate = DateOnly.FromDateTime(DateTime.Today);
                    latestGrade.GradeStatus = GradeStatuses.DISMISSED;
                    latestGrade.SystemStatus = SystemStatuses.DISMISSED;

                    await _context.Assessments
                        .Where(a => a.GradeId == latestGrade.Id)
                        .ExecuteUpdateAsync(s => s
                            .SetProperty(a => a.SystemStatus, SystemStatuses.DISMISSED));

                    await _context.AssessmentResults
                        .Where(ar => (ar.JudgeId == userId || ar.Assessment.GradeId == latestGrade.Id) &&
                                     ar.SystemStatus == SystemStatuses.PENDING)
                        .ExecuteDeleteAsync();
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}