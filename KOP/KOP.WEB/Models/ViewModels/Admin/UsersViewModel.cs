using KOP.Common.Dtos;

namespace KOP.WEB.Models.ViewModels.Admin
{
    public class UsersViewModel
    {
        public List<UserSummaryDto> UserSummaryDtoList { get; set; } = new();
    }
}