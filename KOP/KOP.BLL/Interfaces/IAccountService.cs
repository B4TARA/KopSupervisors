using KOP.Common.DTOs.AccountDTOs;
using KOP.Common.Interfaces;
using System.Security.Claims;

namespace KOP.BLL.Interfaces
{
    public interface IAccountService
    {
        Task<IBaseResponse<ClaimsIdentity>> Login(AccountDTO accountDTO);
        Task<IBaseResponse<object>> RemindPassword(AccountDTO accountDTO);
    }
}