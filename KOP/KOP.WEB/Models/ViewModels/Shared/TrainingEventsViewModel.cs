using KOP.Common.Dtos.GradeDtos;

namespace KOP.WEB.Models.ViewModels.Shared
{
    public class TrainingEventsViewModel
    {
        public int GradeId { get; set; }
        public List<TrainingEventDto> TrainingEvents { get; set; } = new();
    }
}