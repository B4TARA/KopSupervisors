using KOP.Common.DTOs;

namespace KOP.WEB.Models.ViewModels.Shared
{
    public class MarksViewModel
    {
        public int GradeId { get; set; }
        public List<MarkTypeDTO> MarkTypes { get; set; } = new();
    }
}