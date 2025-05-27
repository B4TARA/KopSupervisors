using KOP.Common.Dtos;
using KOP.Common.Dtos.GradeDtos;

namespace KOP.WEB.Models.ViewModels.Employee
{
    public class EmployeeGradesViewModel
    {
        public string EmployeeName { get; set; } = string.Empty;
        public List<GradeReducedDto> GradeReducedDtoList = new();
    }
}