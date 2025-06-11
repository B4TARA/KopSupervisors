using KOP.BLL.Interfaces;
using KOP.Common.Dtos.GradeDtos;
using KOP.DAL;
using KOP.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace KOP.BLL.Services
{
    public class ProjectService : IProjectService
    {
        private readonly ApplicationDbContext _context;

        public ProjectService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProjectDto>> GetProjectDtoListByGradeId(int gradeId)
        {
            if (gradeId == 0)
                throw new ArgumentException("gradeId cannot be 0", nameof(gradeId));

            var projectDtoList = await _context.Projects
                .AsNoTracking()
                .Where(x => x.GradeId == gradeId)
                .OrderByDescending(x => x.DateOfCreation)
                .Select(x => new ProjectDto
                {
                    Id = x.Id,
                    UserRole = x.UserRole,
                    Name = x.Name,
                    Stage = x.Stage,
                    StartDateTime = x.StartDate.ToDateTime(TimeOnly.MinValue),
                    EndDateTime = x.EndDate.ToDateTime(TimeOnly.MaxValue),
                    SuccessRate = x.SuccessRate,
                    AverageKpi = x.AverageKpi,
                    SP = x.SP,
                })
                .ToListAsync();

            return projectDtoList;
        }

        public async Task EditProject(ProjectDto projectDto)
        {
            if (projectDto == null)
                throw new ArgumentNullException(nameof(projectDto), "projectDto cannot be null.");
            else if (projectDto.Id == 0)
                throw new ArgumentException("projectDto.Id cannot be 0", nameof(projectDto));

            var project = await _context.Projects
                .FirstOrDefaultAsync(x => x.Id == projectDto.Id);

            if (project == null)
                throw new KeyNotFoundException($"project with ID {projectDto.Id} not found.");

            project.UserRole = projectDto.UserRole;
            project.Name = projectDto.Name;
            project.Stage = projectDto.Stage;
            project.StartDate = projectDto.StartDate;
            project.EndDate = projectDto.EndDate;
            project.SuccessRate = projectDto.SuccessRate;
            project.AverageKpi = projectDto.AverageKpi;
            project.SP = projectDto.SP;

            await _context.SaveChangesAsync();
        }

        public async Task CreateProject(ProjectDto projectDto)
        {
            if (projectDto == null)
                throw new ArgumentNullException(nameof(projectDto), "projectDto cannot be null.");

            var project = new Project
            {
                UserRole = projectDto.UserRole,
                Name = projectDto.Name,
                Stage = projectDto.Stage,
                StartDate = projectDto.StartDate,
                EndDate = projectDto.EndDate,
                SuccessRate = projectDto.SuccessRate,
                AverageKpi = projectDto.AverageKpi,
                SP = projectDto.SP
            };

            await _context.Projects.AddAsync(project);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProjectById(int projectId)
        {
            if (projectId == 0)
                throw new ArgumentException("projectId cannot be 0", nameof(projectId));

            var project = await _context.Projects
                .FirstOrDefaultAsync(x => x.Id == projectId);

            if (project == null)
                throw new KeyNotFoundException($"project with ID {projectId} not found.");

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
        }
    }
}