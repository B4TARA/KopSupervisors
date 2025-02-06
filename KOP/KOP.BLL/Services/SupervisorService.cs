using KOP.BLL.Interfaces;
using KOP.Common.Dtos;
using KOP.Common.Enums;
using KOP.Common.Interfaces;
using KOP.DAL.Entities;
using KOP.DAL.Interfaces;

namespace KOP.BLL.Services
{
    public class SupervisorService : ISupervisorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMappingService _mappingService;

        public SupervisorService(IUnitOfWork unitOfWork, IMappingService mappingService)
        {
            _unitOfWork = unitOfWork;
            _mappingService = mappingService;
        }

        private async Task<IBaseResponse<List<UserDto>>> GetSubordinateUsers(Subdivision subdivision)
        {
            try
            {
                var subordinateUsers = new List<UserDto>();

                foreach (var user in subdivision.Users.Where(x => x.SystemRoles.Contains(SystemRoles.Employee)))
                {
                    var userDto = _mappingService.CreateUserDto(user);

                    if (userDto.HasData)
                    {
                        continue;
                    }

                    subordinateUsers.Add(userDto.Data);
                }

                foreach (var childSubdivision in subdivision.Children)
                {
                    var subordinateUsersFromChildSubdivisionRes = await GetSubordinateUsers(childSubdivision);

                    if (subordinateUsersFromChildSubdivisionRes.HasData)
                    {
                        return new BaseResponse<List<UserDto>>()
                        {
                            Description = subordinateUsersFromChildSubdivisionRes.Description,
                            StatusCode = subordinateUsersFromChildSubdivisionRes.StatusCode,
                        };
                    }

                    subordinateUsers.AddRange(subordinateUsersFromChildSubdivisionRes.Data);
                }

                return new BaseResponse<List<UserDto>>()
                {
                    Data = subordinateUsers,
                    StatusCode = StatusCodes.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<List<UserDto>>()
                {
                    Description = $"[SupervisorService.GetSubordinateUsers] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        public async Task<IBaseResponse<IEnumerable<SubdivisionDto>>> GetUserSubordinateSubdivisions(int supervisorId, bool onlySubdivisionsWithPendingUsersGrades)
        {
            try
            {
                var supervisor = await _unitOfWork.Users.GetAsync(x => x.Id == supervisorId, includeProperties: new string[]
                {
                    "SubordinateSubdivisions.Users.Grades",
                    "SubordinateSubdivisions.Children.Users.Grades",
                });

                if (supervisor == null)
                {
                    return new BaseResponse<IEnumerable<SubdivisionDto>>()
                    {
                        Description = $"[SupervisorService.GetUserSubordinateSubdivisions] : Пользователь с id = {supervisorId} не найден",
                        StatusCode = StatusCodes.EntityNotFound,
                    };
                }

                var subdivisionsDtos = new List<SubdivisionDto>();

                foreach (var subdivision in supervisor.SubordinateSubdivisions)
                {
                    var subdivisionDto = await ProcessSubdivision(subdivision, onlySubdivisionsWithPendingUsersGrades);
                    if (subdivisionDto != null)
                    {
                        subdivisionsDtos.Add(subdivisionDto);
                    }
                }

                return new BaseResponse<IEnumerable<SubdivisionDto>>()
                {
                    Data = subdivisionsDtos,
                    StatusCode = StatusCodes.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<IEnumerable<SubdivisionDto>>()
                {
                    Description = $"[SupervisorService.GetUserSubordinateSubdivisions] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        private async Task<SubdivisionDto?> ProcessSubdivision(Subdivision subdivision, bool onlySubdivisionsWithPendingUsersGrades)
        {
            var subdivisionDto = new SubdivisionDto
            {
                Id = subdivision.Id,
                Name = subdivision.Name,
                Users = new List<UserDto>(),
                Children = new List<SubdivisionDto>()
            };

            if(subdivision.NestingLevel == 1)
            {
                subdivisionDto.IsRoot = true;
            }

            foreach (var user in subdivision.Users)
            {
                var userDto = _mappingService.CreateUserDto(user);
                if (userDto.HasData)
                {
                    if (!onlySubdivisionsWithPendingUsersGrades || (userDto.Data.LastGrade != null && userDto.Data.LastGrade.SystemStatus == SystemStatuses.PENDING))
                    {
                        subdivisionDto.Users.Add(userDto.Data);
                    }
                }
            }

            foreach (var child in subdivision.Children)
            {
                var childSubdivisionDto = await ProcessSubdivision(child, onlySubdivisionsWithPendingUsersGrades);
                if (childSubdivisionDto != null)
                {
                    subdivisionDto.Children.Add(childSubdivisionDto);
                }
            }

            return subdivisionDto.Users.Count > 0 || subdivisionDto.Children.Count > 0 ? subdivisionDto : null;
        }
    }
}