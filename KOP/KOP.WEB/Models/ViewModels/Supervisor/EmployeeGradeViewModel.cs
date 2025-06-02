using KOP.Common.Dtos.GradeDtos;

namespace KOP.WEB.Models.ViewModels.Supervisor
{
    public class EmployeeGradeViewModel
    {
        public int SupervisorId { get; set; }
        public GradeExtendedDto Grade { get; set; } = new();
        public bool IsNeedSupervisorGrade { get; set; }
    }
}