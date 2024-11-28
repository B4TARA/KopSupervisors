using KOP.Common.DTOs.GradeDTOs;

namespace KOP.WEB.Models.ViewModels.Shared
{
    public class TrainingEventsViewModel
    {
        public int GradeId { get; set; }
        public List<TrainingEventDTO> TrainingEvents { get; set; } = new();
    }
}