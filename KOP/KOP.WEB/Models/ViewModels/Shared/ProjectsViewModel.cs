using KOP.Common.Dtos.GradeDtos;
using System.ComponentModel.DataAnnotations;

namespace KOP.WEB.Models.ViewModels.Shared
{
    public class ProjectsViewModel
    {
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public int GradeId { get; set; }
        public List<ProjectDto> Projects { get; set; } = new();
    }
}