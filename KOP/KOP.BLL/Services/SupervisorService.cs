using KOP.BLL.Interfaces;
using KOP.Common.DTOs;
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

        private async Task<IBaseResponse<List<EmployeeDTO>>> GetSubordinateUsers(Subdivision subdivision)
        {
            try
            {
                var subordinateUsers = new List<EmployeeDTO>();

                foreach (var user in subdivision.Users.Where(x => x.SystemRoles.Contains(SystemRoles.Employee)))
                {
                    var userDto = _mappingService.CreateUserDto(user);

                    if (userDto.StatusCode != StatusCodes.OK || userDto.Data == null)
                    {
                        continue;
                    }

                    subordinateUsers.Add(userDto.Data);
                }

                foreach (var childSubdivision in subdivision.Children)
                {
                    var subordinateUsersFromChildSubdivisionRes = await GetSubordinateUsers(childSubdivision);

                    if (subordinateUsersFromChildSubdivisionRes.StatusCode != StatusCodes.OK || subordinateUsersFromChildSubdivisionRes.Data == null)
                    {
                        return new BaseResponse<List<EmployeeDTO>>()
                        {
                            Description = subordinateUsersFromChildSubdivisionRes.Description,
                            StatusCode = subordinateUsersFromChildSubdivisionRes.StatusCode,
                        };
                    }

                    subordinateUsers.AddRange(subordinateUsersFromChildSubdivisionRes.Data);
                }

                return new BaseResponse<List<EmployeeDTO>>()
                {
                    Data = subordinateUsers,
                    StatusCode = StatusCodes.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<List<EmployeeDTO>>()
                {
                    Description = $"[SupervisorService.GetSubordinateUsers] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        public async Task<IBaseResponse<List<ModuleDTO>>> GetUserSubordinateSubdivisions(int supervisorId)
        {
            try
            {
                var supervisor = await _unitOfWork.Users.GetAsync(x => x.Id == supervisorId, includeProperties: new string[]
                {
                    "Modules.Employees",
                    "Modules.Children.Employees",
                });

                if (supervisor == null)
                {
                    return new BaseResponse<List<ModuleDTO>>()
                    {
                        Description = $"[SupervisorService.GetUserSubordinateSubdivisions] : Пользователь с id = {supervisorId} не найден",
                        StatusCode = StatusCodes.EntityNotFound,
                    };
                }

                var subdivisionsDtos = new List<ModuleDTO>();

                foreach(var subdivision in supervisor.SubordinateSubdivisions)
                {
                    var subdivisionDto = new ModuleDTO
                    {
                        Id = subdivision.Id,
                        Name = subdivision.Name,
                        IsRoot = true,
                    };

                    foreach (var user in subdivision.Users.Where(x => x.SystemRoles.Contains(SystemRoles.Employee)))
                    {
                        var userDto = _mappingService.CreateUserDto(user);

                        if (userDto.StatusCode != StatusCodes.OK || userDto.Data == null)
                        {
                            continue;
                        }

                        subdivisionDto.Employees.Add(userDto.Data);
                    }

                    foreach (var child in subdivision.Children)
                    {
                        var childSubdivisionsRes = await GetSubordinateSubdivisions(child.Id);

                        if (childSubdivisionsRes.StatusCode != StatusCodes.OK || childSubdivisionsRes.Data == null)
                        {
                            return new BaseResponse<List<ModuleDTO>>()
                            {
                                Description = childSubdivisionsRes.Description,
                                StatusCode = childSubdivisionsRes.StatusCode,
                            };
                        }

                        var childSubdivisionDto = new ModuleDTO
                        {
                            Id = child.Id,
                            Name = child.Name,
                        };

                        foreach (var user in child.Users.Where(x => x.SystemRoles.Contains(SystemRoles.Employee)))
                        {
                            var userDto = _mappingService.CreateUserDto(user);

                            if (userDto.StatusCode != StatusCodes.OK || userDto.Data == null)
                            {
                                continue;
                            }

                            childSubdivisionDto.Employees.Add(userDto.Data);
                        }

                        foreach (var childSubdivision in childSubdivisionsRes.Data)
                        {
                            childSubdivisionDto.Children.Add(childSubdivision);
                        }

                        subdivisionDto.Children.Add(childSubdivisionDto);
                    }

                    subdivisionsDtos.Add(subdivisionDto);
                }
               
                return new BaseResponse<List<ModuleDTO>>()
                {
                    Data = subdivisionsDtos,
                    StatusCode = StatusCodes.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<List<ModuleDTO>>()
                {
                    Description = $"[SupervisorService.GetUserSubordinateSubdivisions] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        private async Task<IBaseResponse<List<ModuleDTO>>> GetSubordinateSubdivisions(int subdivisionId)
        {
            try
            {
                var subdivision = await _unitOfWork.Subdivisions.GetAsync(x => x.Id == subdivisionId, includeProperties: new string[]
                {
                    "Children.Employees",
                });

                if (subdivision == null)
                {
                    return new BaseResponse<List<ModuleDTO>>()
                    {
                        Description = $"[SupervisorService.GetSubordinateSubdivisions] : Модуль с id = {subdivisionId} не найден",
                        StatusCode = StatusCodes.EntityNotFound,
                    };
                }

                var subdivisionsDtos = new List<ModuleDTO>();

                foreach (var childModule in subdivision.Children)
                {
                    var subdivisionDto = new ModuleDTO()
                    {
                        Id = childModule.Id,
                        Name = childModule.Name,
                    };

                    foreach (var user in childModule.Users)
                    {
                        var userDto = _mappingService.CreateUserDto(user);

                        if (userDto.StatusCode != StatusCodes.OK || userDto.Data == null)
                        {
                            continue;
                        }

                        subdivisionDto.Employees.Add(userDto.Data);
                    }

                    var childSubdivisionsRes = await GetSubordinateSubdivisions(childModule.Id);

                    if (childSubdivisionsRes.StatusCode != StatusCodes.OK || childSubdivisionsRes.Data == null)
                    {
                        return new BaseResponse<List<ModuleDTO>>()
                        {
                            Description = childSubdivisionsRes.Description,
                            StatusCode = childSubdivisionsRes.StatusCode,
                        };
                    }

                    subdivisionDto.Children.AddRange(childSubdivisionsRes.Data);

                    subdivisionsDtos.Add(subdivisionDto);
                }

                return new BaseResponse<List<ModuleDTO>>()
                {
                    Data = subdivisionsDtos,
                    StatusCode = StatusCodes.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<List<ModuleDTO>>()
                {
                    Description = $"[SupervisorService.GetSubordinateSubdivisions] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }
    }
}