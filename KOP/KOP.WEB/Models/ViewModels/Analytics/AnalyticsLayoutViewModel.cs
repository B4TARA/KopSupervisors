using KOP.Common.Dtos;

namespace KOP.WEB.Models.ViewModels.Analytics
{
    public class AnalyticsLayoutViewModel
    {
        public List<UserSummaryDto> SubordinateUsers { get; set; } = new();
    }
}