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
        public async Task<User?> GetSupervisorForUser(int userId)
        {
            var user = await _unitOfWork.Users.GetAsync(x => x.Id == userId);
            var parentSubdivision = await _unitOfWork.Subdivisions.GetAsync(x => x.Id == user.ParentSubdivisionId, includeProperties: "Parent");

            if (parentSubdivision == null)
            {
                return null;
            }

            var supervisor = await _unitOfWork.Users.GetAsync(x => x.SystemRoles.Contains(SystemRoles.Supervisor) && x.SubordinateSubdivisions.Contains(parentSubdivision));

            if (supervisor != null)
            {
                return supervisor;
            }

            var rootSubdivision = parentSubdivision.Parent;

            while (rootSubdivision != null)
            {
                supervisor = await _unitOfWork.Users.GetAsync(x => x.SystemRoles.Contains(SystemRoles.Supervisor) && x.SubordinateSubdivisions.Contains(rootSubdivision));

                if (supervisor != null)
                {
                    return supervisor;
                }

                rootSubdivision = rootSubdivision.Parent;
            }

            return null;
        }

        public async Task<IBaseResponse<List<UserDto>>> GetSubordinateUsers(int supervisorId)
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
                    return new BaseResponse<List<UserDto>>()
                    {
                        Description = $"Пользователь с id = {supervisorId} не найден",
                        StatusCode = StatusCodes.EntityNotFound,
                    };
                }

                var allSubordinateUsers = new List<UserDto>();

                foreach (var subdivision in supervisor.SubordinateSubdivisions)
                {
                    var getSubordinateUsersRes = await GetSubordinateUsers(subdivision);

                    if (!getSubordinateUsersRes.HasData)
                    {
                        return new BaseResponse<List<UserDto>>()
                        {
                            Description = getSubordinateUsersRes.Description,
                            StatusCode = getSubordinateUsersRes.StatusCode,
                        };
                    }

                    allSubordinateUsers.AddRange(getSubordinateUsersRes.Data);
                }

                return new BaseResponse<List<UserDto>>()
                {
                    Data = allSubordinateUsers,
                    StatusCode = StatusCodes.OK,
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

        public async Task<IBaseResponse<object>> ApproveEmployeeGrade(int gradeId)
        {
            try
            {
                var grade = await _unitOfWork.Grades.GetAsync(x => x.Id == gradeId, includeProperties: "Assessments.AssessmentResults");
                if (grade is null)
                {
                    return new BaseResponse<object>()
                    {
                        Description = $"Оценка с ID = {gradeId} не найдена",
                        StatusCode = StatusCodes.EntityNotFound,
                    };
                }

                grade.GradeStatus = GradeStatuses.COMPLETED;
                grade.SystemStatus = SystemStatuses.COMPLETED;

                foreach (var assessment in grade.Assessments)
                {
                    var pendingAssessmentResults = assessment.AssessmentResults.Where(x => x.SystemStatus == SystemStatuses.PENDING);
                    foreach (var result in pendingAssessmentResults)
                    {
                        _unitOfWork.AssessmentResults.Remove(result);
                    }

                    assessment.SystemStatus = SystemStatuses.COMPLETED;
                }

                _unitOfWork.Grades.Update(grade);
                await _unitOfWork.CommitAsync();

                return new BaseResponse<object>()
                {
                    StatusCode = StatusCodes.OK,
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return new BaseResponse<object>()
                {
                    Description = $"[SupervisorService.ApproveEmployeeGrade] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        private async Task<IBaseResponse<List<UserDto>>> GetSubordinateUsers(Subdivision subdivision)
        {
            try
            {
                var subordinateUsers = new List<UserDto>();

                foreach (var user in subdivision.Users.Where(x => x.SystemRoles.Contains(SystemRoles.Employee)))
                {
                    var userDto = _mappingService.CreateUserDto(user);

                    if (!userDto.HasData)
                    {
                        continue;
                    }

                    subordinateUsers.Add(userDto.Data);
                }

                foreach (var childSubdivision in subdivision.Children)
                {
                    var subordinateUsersFromChildSubdivisionRes = await GetSubordinateUsers(childSubdivision);

                    if (!subordinateUsersFromChildSubdivisionRes.HasData)
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

        public async Task<IBaseResponse<IEnumerable<SubdivisionDto>>> GetUserSubordinateSubdivisions(int supervisorId)
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
                    var subdivisionDto = await ProcessSubdivision(subdivision);
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

        private async Task<SubdivisionDto?> ProcessSubdivision(Subdivision subdivision)
        {
            var subdivisionDto = new SubdivisionDto
            {
                Id = subdivision.Id,
                Name = subdivision.Name,
                Users = new List<UserDto>(),
                Children = new List<SubdivisionDto>()
            };

            if (subdivision.NestingLevel == 1)
            {
                subdivisionDto.IsRoot = true;
            }

            foreach (var user in subdivision.Users)
            {
                var userDto = _mappingService.CreateUserDto(user);
                if (userDto.HasData)
                {
                    subdivisionDto.Users.Add(userDto.Data);
                }
            }

            foreach (var child in subdivision.Children)
            {
                var childSubdivisionDto = await ProcessSubdivision(child);
                if (childSubdivisionDto != null)
                {
                    subdivisionDto.Children.Add(childSubdivisionDto);
                }
            }

            return subdivisionDto.Users.Count > 0 || subdivisionDto.Children.Count > 0 ? subdivisionDto : null;
        }
    }
}