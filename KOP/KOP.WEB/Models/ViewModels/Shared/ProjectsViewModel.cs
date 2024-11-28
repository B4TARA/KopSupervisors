using KOP.Common.DTOs.GradeDTOs;

namespace KOP.WEB.Models.ViewModels.Shared
{
    public class ProjectsViewModel
    {
        public int GradeId { get; set; }
        public List<ProjectDTO> Projects { get; set; } = new();
    }
}