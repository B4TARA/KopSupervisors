using System.Security.Claims;
using KOP.Common.Dtos.AccountDtos;
using KOP.Common.Interfaces;

namespace KOP.BLL.Interfaces
{
    public interface IAccountService
    {
        Task<IBaseResponse<ClaimsIdentity>> Login(LoginDto dto);
        Task<IBaseResponse<object>> RemindPassword(LoginDto dto);
    }
}