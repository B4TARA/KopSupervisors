using KOP.Common.DTOs.GradeDTOs;

namespace KOP.WEB.Models.ViewModels.Employee
{
    public class GradeTypeViewModel
    {
        public List<GradeDTO> Grades { get; set; } = new();
    }
}