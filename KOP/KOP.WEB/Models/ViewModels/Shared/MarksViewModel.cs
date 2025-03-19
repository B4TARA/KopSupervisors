using KOP.Common.Dtos.GradeDtos;

namespace KOP.WEB.Models.ViewModels.Shared
{
    public class MarksViewModel
    {
        public int GradeId { get; set; }
        public int SelectedUserId { get; set; }
        public List<MarkTypeDto> MarkTypes { get; set; } = new();
        public bool IsFinalized { get; set; }

        public bool ViewAccess { get; set; }
        public bool EditAccess { get; set; }
    }
}