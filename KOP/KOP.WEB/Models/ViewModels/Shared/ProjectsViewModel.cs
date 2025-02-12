using KOP.Common.Dtos.GradeDtos;

namespace KOP.WEB.Models.ViewModels.Shared
{
    public class ProjectsViewModel
    {
        public int GradeId { get; set; }
        public List<ProjectDto> Projects { get; set; } = new();
        public bool IsFinalized { get; set; }

        public bool ViewAccess { get; set; }
        public bool EditAccess { get; set; }
    }
}