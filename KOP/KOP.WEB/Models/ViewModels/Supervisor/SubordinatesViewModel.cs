using KOP.Common.Dtos;

namespace KOP.WEB.Models.ViewModels.Supervisor
{
    public class SubordinatesViewModel
    {
        public int SupervisorId { get; set; }
        public List<SubdivisionDto> Subdivisions { get; set; } = new();
    }
}