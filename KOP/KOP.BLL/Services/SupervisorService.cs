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

        // Вспомогательный метод получения подчиненных сотрудников для рекурсии
        private async Task<IBaseResponse<List<EmployeeDTO>>> GetSubordinateEmployees(Module module)
        {
            try
            {
                var subordinateEmployees = new List<EmployeeDTO>();

                // Получаем подчиненных сотрудников текущего модуля
                foreach (var employee in module.Employees.Where(x => x.SystemRoles.Contains(SystemRoles.Employee)))
                {
                    var employeeDTO = _mappingService.CreateEmployeeDTO(employee);

                    if (employeeDTO.StatusCode != StatusCodes.OK || employeeDTO.Data == null)
                    {
                        continue;
                    }

                    subordinateEmployees.Add(employeeDTO.Data);
                }

                // Получаем дочерние модули
                foreach (var childModule in module.Children)
                {
                    var childSubordinateEmployees = await GetSubordinateEmployees(childModule);

                    if (childSubordinateEmployees.StatusCode != StatusCodes.OK || childSubordinateEmployees.Data == null)
                    {
                        return new BaseResponse<List<EmployeeDTO>>()
                        {
                            Description = childSubordinateEmployees.Description,
                            StatusCode = childSubordinateEmployees.StatusCode,
                        };
                    }

                    subordinateEmployees.AddRange(childSubordinateEmployees.Data);
                }

                return new BaseResponse<List<EmployeeDTO>>()
                {
                    Data = subordinateEmployees,
                    StatusCode = StatusCodes.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<List<EmployeeDTO>>()
                {
                    Description = $"[SupervisorService.GetSubordinateEmployees] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        // Получить все подчиненные модули
        public async Task<IBaseResponse<ModuleDTO>> GetAllSubordinateModules(int supervisorId)
        {
            try
            {
                // Получаем руководителя по идентификатору
                var supervisor = await _unitOfWork.Employees.GetAsync(x => x.Id == supervisorId, includeProperties: new string[]
                {
                    "Module.Employees",
                    "Module.Children.Employees",
                });

                if (supervisor == null)
                {
                    return new BaseResponse<ModuleDTO>()
                    {
                        Description = $"[SupervisorService.GetSubordinates] : Пользователь с id = {supervisorId} не найден",
                        StatusCode = StatusCodes.EntityNotFound,
                    };
                }

                var moduleDTO = new ModuleDTO
                {
                    Id = supervisor.Module.Id,
                    Name = supervisor.Module.Name,
                    IsRoot = true,
                };

                // Получаем подчиненных сотрудников такого же модуля
                foreach (var employee in supervisor.Module.Employees.Where(x => x.SystemRoles.Contains(SystemRoles.Employee)))
                {
                    var employeeDTO = _mappingService.CreateEmployeeDTO(employee);

                    if (employeeDTO.StatusCode != StatusCodes.OK || employeeDTO.Data == null)
                    {
                        continue;
                    }

                    moduleDTO.Employees.Add(employeeDTO.Data);
                }

                foreach (var child in supervisor.Module.Children)
                {
                    var response = await GetSubordinateModules(child.Id);

                    if (response.StatusCode != StatusCodes.OK || response.Data == null)
                    {
                        return new BaseResponse<ModuleDTO>()
                        {
                            Description = response.Description,
                            StatusCode = response.StatusCode,
                        };
                    }

                    var childModuleDTO = new ModuleDTO
                    {
                        Id = child.Id,
                        Name = child.Name,
                    };

                    foreach (var employee in child.Employees.Where(x => x.SystemRoles.Contains(SystemRoles.Employee)))
                    {
                        var employeeDTO = _mappingService.CreateEmployeeDTO(employee);

                        if (employeeDTO.StatusCode != StatusCodes.OK || employeeDTO.Data == null)
                        {
                            continue;
                        }

                        childModuleDTO.Employees.Add(employeeDTO.Data);
                    }

                    foreach (var childModule in response.Data)
                    {
                        childModuleDTO.Children.Add(childModule);
                    }

                    moduleDTO.Children.Add(childModuleDTO);
                }

                return new BaseResponse<ModuleDTO>()
                {
                    Data = moduleDTO,
                    StatusCode = StatusCodes.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<ModuleDTO>()
                {
                    Description = $"[SupervisorService.GetAllSubordinateModules] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        // Вспомогательный метод получения дочерних модулей для рекурсии
        private async Task<IBaseResponse<List<ModuleDTO>>> GetSubordinateModules(int moduleId)
        {
            try
            {
                // Получаем модуль по идентификатору
                var module = await _unitOfWork.Modules.GetAsync(x => x.Id == moduleId, includeProperties: new string[]
                {
                    "Children.Employees",
                });

                if (module == null)
                {
                    return new BaseResponse<List<ModuleDTO>>()
                    {
                        Description = $"[SupervisorService.GetSubordinateModules] : Модуль с id = {moduleId} не найден",
                        StatusCode = StatusCodes.EntityNotFound,
                    };
                }

                var moduleDTOs = new List<ModuleDTO>();

                // Получаем дочерние модули
                foreach (var childModule in module.Children)
                {
                    var moduleDTO = new ModuleDTO()
                    {
                        Id = childModule.Id,
                        Name = childModule.Name,
                    };

                    foreach (var employee in childModule.Employees)
                    {
                        var employeeDTO = _mappingService.CreateEmployeeDTO(employee);

                        if (employeeDTO.StatusCode != StatusCodes.OK || employeeDTO.Data == null)
                        {
                            continue;
                        }

                        moduleDTO.Employees.Add(employeeDTO.Data);
                    }

                    var response = await GetSubordinateModules(childModule.Id);

                    if (response.StatusCode != StatusCodes.OK || response.Data == null)
                    {
                        return new BaseResponse<List<ModuleDTO>>()
                        {
                            Description = response.Description,
                            StatusCode = response.StatusCode,
                        };
                    }

                    moduleDTO.Children.AddRange(response.Data);

                    moduleDTOs.Add(moduleDTO);
                }

                return new BaseResponse<List<ModuleDTO>>()
                {
                    Data = moduleDTOs,
                    StatusCode = StatusCodes.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<List<ModuleDTO>>()
                {
                    Description = $"[SupervisorService.GetSubordinateModules] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }
    }
}