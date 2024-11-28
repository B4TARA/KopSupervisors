using KOP.Common.DTOs.GradeDTOs;

namespace KOP.WEB.Models.ViewModels.Shared
{
    public class KpisViewModel
    {
        public int GradeId { get; set; }
        public string? KpisConclusion { get; set; }
        public List<KpiDTO> Kpis { get; set; } = new();
    }
}