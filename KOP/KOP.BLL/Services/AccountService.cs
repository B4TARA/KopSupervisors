using System.Security.Claims;
using KOP.BLL.Interfaces;
using KOP.Common.Dtos;
using KOP.Common.Dtos.AccountDtos;
using KOP.Common.Enums;
using KOP.Common.Interfaces;
using KOP.DAL.Entities;
using KOP.DAL.Interfaces;

namespace KOP.BLL.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AccountService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IBaseResponse<ClaimsIdentity>> Login(LoginDto dto)
        {
            try
            {
                var user = await _unitOfWork.Users.GetAsync(x => x.Login == dto.Login);

                if (user is null)
                {
                    return new BaseResponse<ClaimsIdentity>()
                    {
                        StatusCode = StatusCodes.EntityNotFound,
                        Description = "Пользователь не найден",
                    };
                }
                else if (dto.Password != user.Password)
                {
                    return new BaseResponse<ClaimsIdentity>()
                    {
                        StatusCode = StatusCodes.IncorrectPassword,
                        Description = "Неверный пароль"
                    };
                }
                else if (user.IsSuspended)
                {
                    return new BaseResponse<ClaimsIdentity>()
                    {
                        StatusCode = StatusCodes.UserIsSuspended,
                        Description = "Учетная запись заблокирована"
                    };
                }

                var authenticationResult = Authenticate(user);

                return new BaseResponse<ClaimsIdentity>()
                {
                    Data = authenticationResult,
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

        private ClaimsIdentity Authenticate(User user)
        {
            var claims = new List<Claim>
            {
                new Claim("Id", user.Id.ToString()),
                new Claim("ImagePath", user.ImagePath),
                new Claim("FullName", user.FullName),
            };

            claims.AddRange(GetRoleClaims(user));

            return new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
        }

        private IEnumerable<Claim> GetRoleClaims(User employee)
        {
            var rolesClaims = new List<Claim>();

            foreach (var systemRole in employee.SystemRoles)
            {
                rolesClaims.Add(new Claim(ClaimTypes.Role, systemRole.ToString()));
            }

            return rolesClaims;
        }

        public async Task<IBaseResponse<object>> RemindPassword(LoginDto dto)
        {
            try
            {
                var user = await _unitOfWork.Users.GetAsync(x => x.Login == dto.Login);

                if (user is null)
                {
                    return new BaseResponse<object>()
                    {
                        StatusCode = StatusCodes.EntityNotFound,
                        Description = "Пользователь не найден",
                    };
                }

                //
                // Тут нужно Реализовать логику отправки уведомления на почту
                //

                return new BaseResponse<object>()
                {
                    StatusCode = StatusCodes.OK,
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
