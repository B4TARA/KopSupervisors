using KOP.Common.Dtos.UserDtos;

namespace KOP.WEB.Models.ViewModels.Admin
{
    public class UsersViewModel
    {
        public List<UserReducedDto> UserSummaryDtoList { get; set; } = new();
    }
}