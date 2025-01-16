using KOP.Common.DTOs.GradeDTOs;
using KOP.Common.Enums;

namespace KOP.WEB.Models.ViewModels.Supervisor
{
    public class EmployeeGradeLayoutViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Position { get; set; }
        public string Subdivision { get; set; }
        public string GradeGroup { get; set; }
        public string WorkPeriod { get; set; }
        public DateOnly ContractEndDate { get; set; }
        public string ImagePath { get; set; }
        public GradeDTO? LastGrade { get; set; }
    }
}