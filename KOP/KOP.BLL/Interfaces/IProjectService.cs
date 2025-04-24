using KOP.Common.Dtos.GradeDtos;

namespace KOP.BLL.Interfaces
{
    public interface IProjectService
    {
        Task<List<ProjectDto>> GetProjectDtoListByGradeId(int gradeId);
        Task EditProject(ProjectDto projectDto);
        Task CreateProject(ProjectDto projectDto);
        Task DeleteProjectById(int projectId);
    }
}