using KOP.Common.DTOs.AccountDTOs;
using KOP.Common.Interfaces;
using System.Security.Claims;

namespace KOP.BLL.Interfaces
{
    public interface IAccountService
    {
        Task<IBaseResponse<ClaimsIdentity>> Login(LoginDTO dto);
        Task<IBaseResponse<object>> RemindPassword(LoginDTO dto);
    }
}