using DocumentFormat.OpenXml.Drawing;
using KOP.BLL.Interfaces;
using KOP.Common.Dtos;
using KOP.Common.Dtos.AccountDtos;
using KOP.Common.Enums;
using KOP.Common.Interfaces;
using KOP.DAL.Entities;
using KOP.DAL.Interfaces;
using KOP.EmailService;
using System.Security.Claims;

namespace KOP.BLL.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;

        public AccountService(IUnitOfWork unitOfWork, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
        }

        public async Task<IBaseResponse<ClaimsIdentity>> Login(LoginDto dto)
        {
            try
            {
                dto.Login = dto.Login.Trim();
                dto.Password = dto.Password.Trim();

                var user = await _unitOfWork.Users.GetAsync(x => x.Login == dto.Login);
                if (user == null)
                {
                    return new BaseResponse<ClaimsIdentity>()
                    {
                        StatusCode = StatusCodes.EntityNotFound,
                        Description = "Неверный логин или пароль. Убедитесь, что вводите учетные данные MTSpace Mobile",
                    };
                }
                else if (dto.Password != user.Password)
                {
                    return new BaseResponse<ClaimsIdentity>()
                    {
                        StatusCode = StatusCodes.IncorrectPassword,
                        Description = "Неверный логин или пароль. Убедитесь, что вводите учетные данные MTSpace Mobile"
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

                user.LastLogin = DateTime.UtcNow;

                _unitOfWork.Users.Update(user);
                await _unitOfWork.CommitAsync();

                var authenticationResult = Authenticate(user);

                return new BaseResponse<ClaimsIdentity>()
                {
                    Data = authenticationResult,
                    StatusCode = StatusCodes.OK,
                    Description = "Успешный вход"
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

                if (user == null)
                {
                    return new BaseResponse<object>()
                    {
                        StatusCode = StatusCodes.EntityNotFound,
                        Description = "Пользователь не найден. Пожалуйста, проверьте введенные данные",
                    };
                }

                var message = new Message([user.Email], "Учетные данные", $"Логин - {user.Login}, Пароль - {user.Password}", user.FullName);
                await _emailSender.SendEmailAsync(message);

                return new BaseResponse<object>()
                {
                    StatusCode = StatusCodes.OK,
                    Description = "Данные успешно отправлены на вашу почту."
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