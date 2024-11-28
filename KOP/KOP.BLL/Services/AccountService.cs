using KOP.BLL.Interfaces;
using KOP.Common.DTOs;
using KOP.Common.DTOs.AccountDTOs;
using KOP.Common.Enums;
using KOP.Common.Interfaces;
using KOP.DAL.Entities;
using KOP.DAL.Interfaces;
using System.Security.Claims;

namespace KOP.BLL.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AccountService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IBaseResponse<ClaimsIdentity>> Login(AccountDTO accountDTO)
        {
            try
            {
                var employee = await _unitOfWork.Employees.GetAsync(x => x.Login == accountDTO.Login);

                if (employee == null)
                {
                    return new BaseResponse<ClaimsIdentity>()
                    {
                        Description = "Пользователь не найден"
                    };
                }

                if (accountDTO.Password != employee.Password)
                {
                    return new BaseResponse<ClaimsIdentity>()
                    {
                        Description = "Неверный пароль"
                    };
                }

                if (employee.IsSuspended)
                {
                    return new BaseResponse<ClaimsIdentity>()
                    {
                        Description = "Учетная запись заблокирована"
                    };
                }

                var result = Authenticate(employee);

                return new BaseResponse<ClaimsIdentity>()
                {
                    Data = result,
                    StatusCode = StatusCodes.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<ClaimsIdentity>()
                {
                    Description = $"[AccountService.Login] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError
                };
            }
        }

        private ClaimsIdentity Authenticate(Employee employee)
        {
            var claims = new List<Claim>
            {
                new Claim("Id", employee.Id.ToString()),
                new Claim("ImagePath", employee.ImagePath),
                new Claim("FullName", employee.FullName),
            };

            // Вызов метода для добавления соответствующих ролей
            claims.AddRange(GetRoleClaims(employee));

            return new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
        }

        private IEnumerable<Claim> GetRoleClaims(Employee employee)
        {
            var roleClaims = new List<Claim>
            {
                // Добавляем роль Employee для всех сотрудников
                // new Claim(ClaimTypes.Role, SystemRoles.Employee.ToString())
            };

            foreach(var systemRole in employee.SystemRoles)
            {
                roleClaims.Add(new Claim(ClaimTypes.Role, systemRole.ToString()));
            }

            return roleClaims;
        }

        public async Task<IBaseResponse<object>> RemindPassword(AccountDTO accountDTO)
        {
            try
            {
                var userToRemindPassword = await _unitOfWork.Employees.GetAsync(x => x.Login == accountDTO.Login);

                if (userToRemindPassword == null)
                {
                    return new BaseResponse<object>()
                    {
                        StatusCode = StatusCodes.EntityNotFound,
                    };
                }

                // Реализовать логику отправки уведомления на почту

                return new BaseResponse<object>()
                {
                    Description = "Данные высланы Вам на почту"
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<object>()
                {
                    Description = $"[AccountService.RemindPassword] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError
                };
            }
        }
    }
}
