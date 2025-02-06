using KOP.BLL.Interfaces;
using KOP.Common.Dtos;
using KOP.Common.Dtos.AssessmentDtos;
using KOP.Common.Enums;
using KOP.Common.Interfaces;
using KOP.DAL.Entities.AssessmentEntities;
using KOP.DAL.Interfaces;

namespace KOP.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAssessmentService _assessmentService;
        private readonly IMappingService _mappingService;

        public UserService(IUnitOfWork unitOfWork, IAssessmentService assessmentService, IMappingService mappingService)
        {
            _unitOfWork = unitOfWork;
            _assessmentService = assessmentService;
            _mappingService = mappingService;
        }

        public async Task<IBaseResponse<UserDto>> GetUser(int id)
        {
            try
            {
                var user = await _unitOfWork.Users.GetAsync(x => x.Id == id, includeProperties: new string[]
                {
                    "Grades.Qualification",
                    "Grades.ValueJudgment",
                    "Grades.Marks",
                    "Grades.Kpis",
                    "Grades.Projects",
                    "Grades.StrategicTasks",
                    "Grades.TrainingEvents",
                });

                if (user == null)
                {
                    return new BaseResponse<UserDto>()
                    {
                        Description = $"[UserService.GetUser] : Пользователь с id = {id} не найден",
                        StatusCode = StatusCodes.EntityNotFound,
                    };
                }

                var userDto = _mappingService.CreateUserDto(user);

                if (userDto.StatusCode != StatusCodes.OK || userDto.Data == null)
                {
                    return new BaseResponse<UserDto>()
                    {
                        Description = userDto.Description,
                        StatusCode = userDto.StatusCode,
                    };
                }

                return new BaseResponse<UserDto>()
                {
                    Data = userDto.Data,
                    StatusCode = StatusCodes.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<UserDto>()
                {
                    Description = $"[UserService.GetUser] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        public async Task<IBaseResponse<List<AssessmentDto>>> GetUserLastAssessmentsOfEachAssessmentType(int userId, int supervisorId)
        {
            try
            {
                var user = await _unitOfWork.Users.GetAsync(x => x.Id == userId, includeProperties: new string[]
                {
                    "Assessments.AssessmentType"
                });

                if (user == null)
                {
                    return new BaseResponse<List<AssessmentDto>>()
                    {
                        Description = $"[UserService.GetUserLastAssessmentsOfEachType] : Пользователь с id = {userId} не найден",
                        StatusCode = StatusCodes.EntityNotFound,
                    };
                }

                var userAssessmentTypes = user.Assessments.GroupBy(x => x.AssessmentType);
                var userLastAssessmentsOfEachType = new List<AssessmentDto>();

                foreach (var assessmentType in userAssessmentTypes)
                {
                    var lastAssessment = assessmentType.OrderByDescending(x => x.DateOfCreation).First();

                    var lastAssessmentDto = new AssessmentDto
                    {
                        Id = lastAssessment.Id,
                        UserId = userId,
                        Number = lastAssessment.Number,
                        AssessmentTypeName = assessmentType.Key.Name,
                    };

                    var isActiveAssessmentRes = await _assessmentService.IsActiveAssessment(supervisorId, userId, lastAssessment.Id);

                    if (isActiveAssessmentRes.StatusCode != StatusCodes.OK)
                    {
                        return new BaseResponse<List<AssessmentDto>>()
                        {
                            Description = isActiveAssessmentRes.Description,
                            StatusCode = isActiveAssessmentRes.StatusCode,
                        };
                    }

                    lastAssessmentDto.IsActiveAssessment = isActiveAssessmentRes.Data;

                    userLastAssessmentsOfEachType.Add(lastAssessmentDto);
                }

                return new BaseResponse<List<AssessmentDto>>()
                {
                    Data = userLastAssessmentsOfEachType,
                    StatusCode = StatusCodes.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<List<AssessmentDto>>()
                {
                    Description = $"[UserService.GetUserLastAssessmentsOfEachType] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        public async Task<IBaseResponse<AssessmentResultDto>> GetUserSelfAssessmentResultByAssessment(int userId, int assessmentId)
        {
            try
            {
                var selfAssessmentResultDto = new AssessmentResultDto();
                var assessment = await _unitOfWork.Assessments.GetAsync(x => x.Id == assessmentId);
                var assessmentType = await _unitOfWork.AssessmentTypes.GetAsync(x => x.Id == assessment.AssessmentTypeId);

                var userSelfAssessmentResult = await _unitOfWork.AssessmentResults.GetAsync(x => x.JudgeId == userId && x.AssessmentId == assessmentId, includeProperties: new string[]
                {
                    "AssessmentResultValues"
                });

                if (userSelfAssessmentResult == null)
                {
                    selfAssessmentResultDto.SystemStatus = SystemStatuses.NOT_EXIST;

                    return new BaseResponse<AssessmentResultDto>()
                    {
                        Data = selfAssessmentResultDto,
                        StatusCode = StatusCodes.OK
                    };
                }
                else if (userSelfAssessmentResult.SystemStatus == SystemStatuses.COMPLETED)
                {
                    var userSelfAssessmentResultValues = userSelfAssessmentResult.AssessmentResultValues;

                    foreach (var value in userSelfAssessmentResultValues)
                    {
                        selfAssessmentResultDto.Values.Add(new AssessmentResultValueDto
                        {
                            Value = value.Value,
                            AssessmentMatrixRow = value.AssessmentMatrixRow,
                        });

                        selfAssessmentResultDto.AverageValues.Add(new AssessmentResultValueDto
                        {
                            Value = 0,
                            AssessmentMatrixRow = value.AssessmentMatrixRow,
                        });
                    }

                    var userColleaguesAssessmentResults = await _unitOfWork.AssessmentResults.GetAllAsync(x => x.JudgeId != userId && x.AssessmentId == assessmentId, includeProperties: new string[]
                    {
                        "AssessmentResultValues"
                    });

                    foreach (var assessmentResult in userColleaguesAssessmentResults)
                    {
                        var assessmentResultValues = assessmentResult.AssessmentResultValues;

                        foreach (var value in assessmentResultValues)
                        {
                            selfAssessmentResultDto.AverageValues.First(x => x.AssessmentMatrixRow == value.AssessmentMatrixRow).Value += value.Value;
                        }
                    }

                    foreach (var averageValue in selfAssessmentResultDto.AverageValues)
                    {
                        var sum = selfAssessmentResultDto.AverageValues.First(x => x.AssessmentMatrixRow == averageValue.AssessmentMatrixRow).Value;
                        var average = Math.Round((double)sum / userColleaguesAssessmentResults.Count(), 1);

                        selfAssessmentResultDto.AverageValues.First(x => x.AssessmentMatrixRow == averageValue.AssessmentMatrixRow).Value = average;
                        selfAssessmentResultDto.AverageResult += average;
                    }
                }

                selfAssessmentResultDto.Judge = new UserDto
                {
                    Id = userId,
                };

                selfAssessmentResultDto.Judged = new UserDto
                {
                    Id = userId,
                };

                selfAssessmentResultDto.Id = userSelfAssessmentResult.Id;
                selfAssessmentResultDto.AssessmentId = userSelfAssessmentResult.AssessmentId;
                selfAssessmentResultDto.SystemStatus = userSelfAssessmentResult.SystemStatus;

                var assessmentMatrix = await _unitOfWork.AssessmentMatrices.GetAsync(x => x.Id == assessmentType.AssessmentMatrixId, includeProperties: new string[]
                {
                    "Elements"
                });

                selfAssessmentResultDto.MinValue = assessmentMatrix.MinAssessmentMatrixResultValue;
                selfAssessmentResultDto.MaxValue = assessmentMatrix.MaxAssessmentMatrixResultValue;

                var assessmentMatrixElementsDtos = new List<AssessmentMatrixElementDto>();

                foreach (var element in assessmentMatrix.Elements)
                {
                    assessmentMatrixElementsDtos.Add(new AssessmentMatrixElementDto
                    {
                        Row = element.Row,
                        Value = element.Value,
                    });
                }

                selfAssessmentResultDto.Elements = assessmentMatrixElementsDtos;
                selfAssessmentResultDto.ElementsByRow = selfAssessmentResultDto.Elements.GroupBy(x => x.Row).ToList();

                return new BaseResponse<AssessmentResultDto>()
                {
                    Data = selfAssessmentResultDto,
                    StatusCode = StatusCodes.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<AssessmentResultDto>()
                {
                    Description = $"[UserService.GetUserSelfAssessmentResultByAssessment] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        public async Task<IBaseResponse<List<AssessmentResultDto>>> GetColleaguesAssessmentResultsForAssessment(int userId)
        {
            try
            {
                var colleaguesAssessmentResultsForAssessment = await _unitOfWork.AssessmentResults.GetAllAsync(x => x.JudgeId == userId && x.JudgedId != userId && x.SystemStatus == SystemStatuses.PENDING);
                var dtos = new List<AssessmentResultDto>();

                foreach (var assessmentResult in colleaguesAssessmentResultsForAssessment)
                {
                    var assessment = await _unitOfWork.Assessments.GetAsync(x => x.Id == assessmentResult.AssessmentId, includeProperties: new string[]
                    {
                        "AssessmentResults.Judged",
                        "AssessmentResults.AssessmentResultValues",
                        "AssessmentType.AssessmentMatrix.Elements"
                    });

                    var assessmentResultDto = new AssessmentResultDto
                    {
                        Id = assessmentResult.Id,
                        AssessmentId = assessmentResult.AssessmentId,
                        SystemStatus = assessmentResult.SystemStatus,
                        Sum = assessmentResult.AssessmentResultValues.Sum(x => x.Value),
                        TypeName = assessment.AssessmentType.Name,
                        MinValue = assessment.AssessmentType.AssessmentMatrix.MinAssessmentMatrixResultValue,
                        MaxValue = assessment.AssessmentType.AssessmentMatrix.MaxAssessmentMatrixResultValue,
                    };

                    assessmentResultDto.Judge = new UserDto
                    {
                        Id = userId,
                    };

                    assessmentResultDto.Judged = new UserDto
                    {
                        Id = assessment.UserId,
                        ImagePath = assessmentResult.Judged.ImagePath,
                        FullName = assessmentResult.Judged.FullName,
                    };

                    foreach (var value in assessmentResult.AssessmentResultValues)
                    {
                        assessmentResultDto.Values.Add(new AssessmentResultValueDto
                        {
                            Value = value.Value,
                            AssessmentMatrixRow = value.AssessmentMatrixRow,
                        });
                    }

                    foreach (var element in assessment.AssessmentType.AssessmentMatrix.Elements)
                    {
                        assessmentResultDto.Elements.Add(new AssessmentMatrixElementDto
                        {
                            Row = element.Row,
                            Value = element.Value,
                        });
                    }

                    assessmentResultDto.ElementsByRow = assessmentResultDto.Elements.GroupBy(x => x.Row).ToList();

                    dtos.Add(assessmentResultDto);
                }

                return new BaseResponse<List<AssessmentResultDto>>()
                {
                    Data = dtos,
                    StatusCode = StatusCodes.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<List<AssessmentResultDto>>()
                {
                    Description = $"[UserService.GetColleaguesAssessmentResultsForAssessment] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        public async Task<IBaseResponse<object>> AssessUser(AssessUserDto assessUserDto)
        {
            try
            {
                var assessmentResult = await _unitOfWork.AssessmentResults.GetAsync(x => x.Id == assessUserDto.AssessmentResultId);

                if (assessmentResult == null)
                {
                    return new BaseResponse<object>()
                    {
                        Description = $"[UserService.AssessUser] : Результат оценки с id = {assessUserDto.AssessmentResultId} не найден",
                        StatusCode = StatusCodes.EntityNotFound,
                    };
                }

                assessmentResult.ResultDate = DateOnly.FromDateTime(DateTime.Today);
                assessmentResult.SystemStatus = SystemStatuses.COMPLETED;

                for (int i = 0; i < assessUserDto.ResultValues.Count(); i++)
                {
                    assessmentResult.AssessmentResultValues.Add(new AssessmentResultValue
                    {
                        Value = Convert.ToInt32(assessUserDto.ResultValues[i]),
                        AssessmentMatrixRow = (i + 1),
                    });
                }

                _unitOfWork.AssessmentResults.Update(assessmentResult);

                // Проверка на условия завершения оценки
                // ПОМЕНЯТЬ ЭТОТ КОСТЫЛЬ !!! (если уже есть в БД оценка одна (руководителя или самооценка), которая завершена
                var secondAssessmentResult = await _unitOfWork.AssessmentResults.GetAsync(x => x.AssessmentId == assessmentResult.AssessmentId && x.SystemStatus == SystemStatuses.COMPLETED);

                if (secondAssessmentResult == null)
                {
                    await _unitOfWork.CommitAsync();

                    return new BaseResponse<object>()
                    {
                        StatusCode = StatusCodes.OK
                    };
                }
                else
                {
                    var assessment = await _unitOfWork.Assessments.GetAsync(x => x.Id == assessmentResult.AssessmentId, includeProperties: new string[]
                    {
                        "AssessmentType",
                    });

                    assessment.SystemStatus = SystemStatuses.COMPLETED;

                    _unitOfWork.Assessments.Update(assessment);

                    await _unitOfWork.CommitAsync();
                }

                return new BaseResponse<object>()
                {
                    StatusCode = StatusCodes.OK
                };
                // // // // // // // // // // //
            }
            catch (Exception ex)
            {
                return new BaseResponse<object>()
                {
                    Description = $"[UserService.AssessUser] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        public async Task<IBaseResponse<AssessmentDto>> GetLastAssessmentByAssessmentType(int userId, int assessmentTypeId)
        {
            try
            {
                var user = await _unitOfWork.Users.GetAsync(x => x.Id == userId, includeProperties: new string[]
                {
                    "Assessments.AssessmentType.AssessmentMatrix.Elements",
                    "Assessments.AssessmentResults.AssessmentResultValues",
                    "Assessments.AssessmentResults.Judge",
                    "Assessments.AssessmentResults.Judged"
                });

                if (user == null)
                {
                    return new BaseResponse<AssessmentDto>()
                    {
                        Description = $"[UserService.GetLastAssessmentByAssessmentType] : Пользователь с id = {userId} не найден",
                        StatusCode = StatusCodes.EntityNotFound,
                    };
                }

                var lastAssessmentByAssessmentType = user.Assessments.Where(x => x.AssessmentTypeId == assessmentTypeId).OrderByDescending(x => x.DateOfCreation).FirstOrDefault();

                if (lastAssessmentByAssessmentType == null)
                {
                    return new BaseResponse<AssessmentDto>()
                    {
                        Description = $"[UserService.GetLastAssessmentByAssessmentType] : Тип с id = {assessmentTypeId} не содержит оценок",
                        StatusCode = StatusCodes.EntityNotFound,
                    };
                }

                var createAssessmentDtoRes = _mappingService.CreateAssessmentDto(lastAssessmentByAssessmentType);

                if (createAssessmentDtoRes.StatusCode != StatusCodes.OK || createAssessmentDtoRes.Data == null)
                {
                    return new BaseResponse<AssessmentDto>()
                    {
                        Description = createAssessmentDtoRes.Description,
                        StatusCode = createAssessmentDtoRes.StatusCode,
                    };
                }

                return new BaseResponse<AssessmentDto>()
                {
                    Data = createAssessmentDtoRes.Data,
                    StatusCode = StatusCodes.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<AssessmentDto>()
                {
                    Description = $"[UserService.GetLastAssessmentByAssessmentType] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }
    }
}