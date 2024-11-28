using KOP.Common.DTOs.GradeDTOs;

namespace KOP.WEB.Models.ViewModels.Shared
{
    public class StrategicTasksViewModel
    {
        public int GradeId { get; set; }
        public string? StrategicTasksConclusion { get; set; }
        public List<StrategicTaskDTO> StrategicTasks { get; set; } = new();
    }
}