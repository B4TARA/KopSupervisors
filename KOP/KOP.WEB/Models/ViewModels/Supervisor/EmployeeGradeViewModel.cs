using KOP.Common.DTOs.GradeDTOs;

namespace KOP.WEB.Models.ViewModels.Supervisor
{
    public class EmployeeGradeViewModel
    {
        public int SupervisorId { get; set; }
        public GradeDTO Grade { get; set; } = new();
        public bool IsNeedSupervisorGrade { get; set; }
    }
}