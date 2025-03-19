using KOP.Common.Dtos.GradeDtos;

namespace KOP.WEB.Models.ViewModels.Shared
{
    public class ProjectsViewModel
    {
        public int SelectedUserId { get; set; }
        public string SelectedUserFullName { get; set; }
        public int GradeId { get; set; }
        public double Qn2 { get; set; }
        public List<ProjectDto> Projects { get; set; } = new();
        public bool IsFinalized { get; set; }

        public bool ViewAccess { get; set; }
        public bool EditAccess { get; set; }
    }
}