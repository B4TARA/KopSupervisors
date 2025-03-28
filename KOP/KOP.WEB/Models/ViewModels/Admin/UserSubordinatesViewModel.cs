using KOP.Common.Dtos;

namespace KOP.WEB.Models.ViewModels.Admin
{
    public class UserSubordinatesViewModel
    {
        public int UserId { get; set; }
        public List<int> SubordinateSubdivisionsIds { get; set; } = new();
        public List<SubdivisionSummaryDto> Subdivisions { get; set; } = new();
    }
}