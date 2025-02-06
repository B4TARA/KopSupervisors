using KOP.Common.Dtos.GradeDtos;

namespace KOP.WEB.Models.ViewModels.Employee
{
    public class GradeTypeViewModel
    {
        public List<GradeDto> Grades { get; set; } = new();
    }
}