using System.Security.Claims;
using KOP.BLL.Interfaces;
using KOP.Common.Dtos;
using KOP.Common.Dtos.AccountDtos;
using KOP.Common.Enums;
using KOP.Common.Interfaces;
using KOP.DAL.Entities;
using KOP.DAL.Interfaces;
using Npgsql;

namespace KOP.BLL.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AccountService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IBaseResponse<ClaimsIdentity>> LoginNow(LoginDto dto)
        {
            try
            {
                var user = await _unitOfWork.Users.GetAsync(x => x.Login == dto.Login && x.Password == dto.Password);
                if (user.IsSuspended)
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
        public async Task<IBaseResponse<ClaimsIdentity>> Login(LoginDto dto)
        {
            try
            {
                var user = await _unitOfWork.Users.GetAsync(x => x.Login == dto.Login && x.Password == dto.Password);

                var existsInKopSupervisorsDatabase = user != null;
                var existsInKopEmployeesDatabase = await UserExistsInKopDatabase(dto.Login, dto.Password);

                if (!existsInKopSupervisorsDatabase && existsInKopEmployeesDatabase)
                {
                    return new BaseResponse<ClaimsIdentity>()
                    {
                        StatusCode = StatusCodes.Redirect,
                    };
                }
                else if (existsInKopSupervisorsDatabase && !existsInKopEmployeesDatabase)
                {
                    if (user.IsSuspended)
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
                        StatusCode = StatusCodes.OK
                    };
                }
                else if (existsInKopSupervisorsDatabase && existsInKopEmployeesDatabase)
                {
                    return new BaseResponse<ClaimsIdentity>()
                    {
                        StatusCode = StatusCodes.UserExistsInMultipleDatabases
                    };
                }
                else
                {
                    return new BaseResponse<ClaimsIdentity>()
                    {
                        StatusCode = StatusCodes.EntityNotFound,
                        Description = "Неверные учетные данные",
                    };
                }
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

        private async Task<bool> UserExistsInKopDatabase(string login, string password)
        {
            var connectionString = "Host=localhost;Database=KOP;Username=postgres;Password=12345";

            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new NpgsqlCommand("SELECT COUNT(*) FROM \"USER_SYSTEM_INFO\" usi  WHERE login = @Login AND password = @Password", connection))
                    {
                        command.Parameters.AddWithValue("@Login", login);
                        command.Parameters.AddWithValue("@Password", password);

                        var count = (long)await command.ExecuteScalarAsync();
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при проверке пользователя: {ex.Message}");
                return false;
            }
        }
    }
}