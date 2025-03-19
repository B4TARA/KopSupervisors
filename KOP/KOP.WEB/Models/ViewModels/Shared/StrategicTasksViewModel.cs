using KOP.Common.Dtos.GradeDtos;

namespace KOP.WEB.Models.ViewModels.Shared
{
    public class StrategicTasksViewModel
    {
        public int GradeId { get; set; }
        public string? Conclusion { get; set; }
        public List<StrategicTaskDto> StrategicTaskDtoList { get; set; } = new();
        public bool IsFinalized { get; set; }

        public bool ViewAccess { get; set; }
        public bool EditAccess { get; set; }
        public bool ConclusionEditAccess { get; set; }
    }
}