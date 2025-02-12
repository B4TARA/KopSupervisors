using KOP.BLL.Interfaces;
using KOP.Common.Dtos;
using KOP.Common.Dtos.AssessmentDtos;
using KOP.Common.Enums;
using KOP.Common.Interfaces;
using KOP.DAL.Interfaces;

namespace KOP.BLL.Services
{
    public class AssessmentService : IAssessmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMappingService _mappingService;

        public AssessmentService(IUnitOfWork unitOfWork, IMappingService mappingService)
        {
            _unitOfWork = unitOfWork;
            _mappingService = mappingService;
        }

        public async Task<IBaseResponse<AssessmentDto>> GetAssessment(int id)
        {
            try
            {
                var assessment = await _unitOfWork.Assessments.GetAsync(x => x.Id == id, includeProperties: new string[]
                {
                    "AssessmentType.AssessmentInterpretations",
                    "AssessmentType.AssessmentMatrix.Elements",
                    "AssessmentResults.AssessmentResultValues",
                    "AssessmentResults.Judge",
                    "User",
                });

                if (assessment == null)
                {
                    return new BaseResponse<AssessmentDto>()
                    {
                        Description = $"[AssessmentService.GetAssessment] : Качественная оценка с id = {id} не найдена",
                        StatusCode = StatusCodes.EntityNotFound,
                    };
                }

                var createAssessmentDtoRes = _mappingService.CreateAssessmentDto(assessment);

                if (!createAssessmentDtoRes.HasData)
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
                    Description = $"[AssessmentService.GetAssessment] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        public async Task<IBaseResponse<AssessmentResultDto>> GetAssessmentResult(int judgeId, int assessmentId)
        {
            try
            {
                var assessmentResult = await _unitOfWork.AssessmentResults.GetAsync(x =>
                    x.AssessmentId == assessmentId && x.JudgeId == judgeId,
                    includeProperties: new string[]
                        {
                            "Judge",
                            "AssessmentResultValues",
                            "Assessment.AssessmentType.AssessmentMatrix.Elements",
                            "Assessment.User",
                        });

                if (assessmentResult == null)
                {
                    return new BaseResponse<AssessmentResultDto>()
                    {
                        Description = $"[AssessmentService.GetAssessmentResult] : Результат качественной оценки не найден",
                        StatusCode = StatusCodes.EntityNotFound,
                    };
                }

                var createAssessmentResultDtoRes = _mappingService.CreateAssessmentResultDto(assessmentResult, assessmentResult.Assessment.AssessmentType);

                if (!createAssessmentResultDtoRes.HasData)
                {
                    return new BaseResponse<AssessmentResultDto>()
                    {
                        Description = createAssessmentResultDtoRes.Description,
                        StatusCode = createAssessmentResultDtoRes.StatusCode,
                    };
                }

                return new BaseResponse<AssessmentResultDto>()
                {
                    Data = createAssessmentResultDtoRes.Data,
                    StatusCode = StatusCodes.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<AssessmentResultDto>()
                {
                    Description = $"[AssessmentService.GetAssessmentResult] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        public async Task<IBaseResponse<AssessmentTypeDto>> GetAssessmentType(int userId, int assessmentTypeId)
        {
            try
            {
                var assessmentType = await _unitOfWork.AssessmentTypes.GetAsync(x =>
                    x.Id == assessmentTypeId,
                    includeProperties: new string[]
                    {
                        "AssessmentMatrix.Elements"
                    });

                if (assessmentType == null)
                {
                    return new BaseResponse<AssessmentTypeDto>()
                    {
                        Description = $"[EmployeeService.GetAssessmentType] : Тип количественной оценки с id = {assessmentTypeId} не найден",
                        StatusCode = StatusCodes.EntityNotFound,
                    };
                }

                var assessments = await _unitOfWork.Assessments.GetAllAsync(x =>
                    x.AssessmentTypeId == assessmentTypeId && x.UserId == userId,
                    includeProperties: new string[]
                    {
                        "AssessmentResults.Judge",
                        "AssessmentResults.Judged",
                        "AssessmentResults.AssessmentResultValues"
                    });

                var assessmentTypeDto = new AssessmentTypeDto
                {
                    Id = assessmentType.Id,
                    Name = assessmentType.Name,
                    UserId = userId,
                };

                foreach (var assessment in assessments.OrderBy(x => x.Number))
                {
                    var assessmentDto = new AssessmentDto
                    {
                        Id = assessment.Id,
                        Number = assessment.Number,
                        UserId = assessment.UserId,
                        SystemStatus = assessment.SystemStatus,
                    };

                    foreach (var assessmentResult in assessment.AssessmentResults.Where(x => x.SystemStatus == SystemStatuses.COMPLETED))
                    {
                        var assessmentResultDto = new AssessmentResultDto
                        {
                            Id = assessmentResult.Id,
                            SystemStatus = assessmentResult.SystemStatus,
                            Sum = assessmentResult.AssessmentResultValues.Sum(x => x.Value),
                        };

                        assessmentResultDto.Judge = new UserDto
                        {
                            Id = assessmentResult.Judge.Id,
                            ImagePath = assessmentResult.Judge.ImagePath,
                            FullName = assessmentResult.Judge.FullName,
                        };

                        assessmentResultDto.Judged = new UserDto
                        {
                            Id = assessment.UserId,
                        };

                        foreach (var assessmentResultValue in assessmentResult.AssessmentResultValues)
                        {
                            assessmentResultDto.Values.Add(new AssessmentResultValueDto
                            {
                                Value = assessmentResultValue.Value,
                                AssessmentMatrixRow = assessmentResultValue.AssessmentMatrixRow,
                            });
                        }

                        foreach (var assessmentMatrixElement in assessmentType.AssessmentMatrix.Elements)
                        {
                            assessmentResultDto.Elements.Add(new AssessmentMatrixElementDto
                            {
                                Row = assessmentMatrixElement.Row,
                                Value = assessmentMatrixElement.Value,
                                HtmlClassName = assessmentMatrixElement.HtmlClassName,
                            });
                        }

                        assessmentResultDto.ElementsByRow = assessmentResultDto.Elements.GroupBy(x => x.Row).OrderBy(x => x.Key).ToList();

                        assessmentDto.AssessmentResults.Add(assessmentResultDto);
                    }

                    assessmentTypeDto.Assessments.Add(assessmentDto);
                }

                return new BaseResponse<AssessmentTypeDto>()
                {
                    Data = assessmentTypeDto,
                    StatusCode = StatusCodes.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<AssessmentTypeDto>()
                {
                    Description = $"[EmployeeService.GetEmployeeAssessmentType] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        public async Task<IBaseResponse<AssessmentSummaryDto>> GetAssessmentSummary(int assessmentId)
        {
            try
            {
                var assessment = await _unitOfWork.Assessments.GetAsync(x => x.Id == assessmentId, includeProperties: new string[]
                {
                    "AssessmentResults.AssessmentResultValues",
                    "AssessmentType.AssessmentMatrix.Elements",
                    "AssessmentType.AssessmentInterpretations",
                    "AssessmentResults.Judge",
                });

                if (assessment is null)
                {
                    return new BaseResponse<AssessmentSummaryDto>()
                    {
                        Description = $"[AssessmentService.GetAssessmentSummary] : Оценка с id = {assessmentId} не найдена",
                        StatusCode = StatusCodes.EntityNotFound,
                    };
                }

                var assessmentSummaryDto = new AssessmentSummaryDto();
                var assessmentMatrixElementsDtos = new List<AssessmentMatrixElementDto>();

                foreach (var element in assessment.AssessmentType.AssessmentMatrix.Elements)
                {
                    assessmentMatrixElementsDtos.Add(new AssessmentMatrixElementDto
                    {
                        Row = element.Row,
                        Value = element.Value,
                        HtmlClassName = element.HtmlClassName,
                    });
                }

                assessmentSummaryDto.Elements = assessmentMatrixElementsDtos;
                assessmentSummaryDto.ElementsByRow = assessmentSummaryDto.Elements.GroupBy(x => x.Row).OrderBy(x => x.Key).ToList();

                var selfAssessmentResult = assessment.AssessmentResults.FirstOrDefault(x => x.JudgeId == assessment.UserId);

                if (selfAssessmentResult != null)
                {
                    var selfAssessmentResultValues = selfAssessmentResult.AssessmentResultValues;

                    foreach (var value in selfAssessmentResultValues)
                    {
                        assessmentSummaryDto.SelfAssessmentResultValues.Add(new AssessmentResultValueDto
                        {
                            Value = value.Value,
                            AssessmentMatrixRow = value.AssessmentMatrixRow,
                        });

                        assessmentSummaryDto.AverageAssessmentResultValues.Add(new AssessmentResultValueDto
                        {
                            Value = 0,
                            AssessmentMatrixRow = value.AssessmentMatrixRow,
                        });
                    }
                }

                var supervisorAssessmentResult = assessment.AssessmentResults.FirstOrDefault(x => x.Judge.SystemRoles.Contains(SystemRoles.Supervisor));

                if (supervisorAssessmentResult != null)
                {
                    var supervisorAssessmentResultValues = supervisorAssessmentResult.AssessmentResultValues;

                    foreach (var value in supervisorAssessmentResultValues)
                    {
                        assessmentSummaryDto.SupervisorAssessmentResultValues.Add(new AssessmentResultValueDto
                        {
                            Value = value.Value,
                            AssessmentMatrixRow = value.AssessmentMatrixRow,
                        });
                    }
                }

                var completedAssessmentResults = assessment.AssessmentResults.Where(x => x.SystemStatus == SystemStatuses.COMPLETED);

                if (assessment.AssessmentType.SystemAssessmentType == SystemAssessmentTypes.СorporateСompetencies)
                {
                    completedAssessmentResults = completedAssessmentResults.Where(x => x.JudgeId != assessment.UserId);
                }

                foreach (var result in completedAssessmentResults)
                {
                    var assessmentResultValues = result.AssessmentResultValues;

                    foreach (var value in assessmentResultValues)
                    {
                        assessmentSummaryDto.AverageAssessmentResultValues.First(x => x.AssessmentMatrixRow == value.AssessmentMatrixRow).Value += value.Value;
                        assessmentSummaryDto.SumResult += value.Value;
                    }
                }

                foreach (var value in assessmentSummaryDto.AverageAssessmentResultValues)
                {
                    var sum = assessmentSummaryDto.AverageAssessmentResultValues.First(x => x.AssessmentMatrixRow == value.AssessmentMatrixRow).Value;
                    var average = sum / completedAssessmentResults.Count();

                    assessmentSummaryDto.AverageAssessmentResultValues.First(x => x.AssessmentMatrixRow == value.AssessmentMatrixRow).Value = average;
                    assessmentSummaryDto.AverageResult += average;
                }

                foreach (var interpretation in assessment.AssessmentType.AssessmentInterpretations)
                {
                    var createAssessmentInterpretationDtoRes = _mappingService.CreateAssessmentInterpretationDto(interpretation);

                    if (!createAssessmentInterpretationDtoRes.HasData)
                    {
                        return new BaseResponse<AssessmentSummaryDto>()
                        {
                            Description = createAssessmentInterpretationDtoRes.Description,
                            StatusCode = createAssessmentInterpretationDtoRes.StatusCode,
                        };
                    }

                    var interpretationDto = createAssessmentInterpretationDtoRes.Data;

                    if (assessmentSummaryDto.AverageResult >= interpretationDto.MinValue && assessmentSummaryDto.AverageResult <= interpretationDto.MaxValue)
                    {
                        assessmentSummaryDto.AverageAssessmentInterpretation = interpretationDto;
                    }

                    assessmentSummaryDto.AssessmentTypeInterpretations.Add(interpretationDto);
                }

                return new BaseResponse<AssessmentSummaryDto>()
                {
                    Data = assessmentSummaryDto,
                    StatusCode = StatusCodes.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<AssessmentSummaryDto>()
                {
                    Description = $"[AssessmentService.GetAssessmentSummary] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        public async Task<IBaseResponse<bool>> IsActiveAssessment(int judgeId, int judgedId, int? assessmentId)
        {
            try
            {
                var pendingAssessmentResults = await _unitOfWork.AssessmentResults.GetAllAsync(x =>
                x.JudgeId == judgeId &&
                x.Assessment.UserId == judgedId &&
                x.SystemStatus == SystemStatuses.PENDING);

                if (assessmentId.HasValue)
                {
                    pendingAssessmentResults = pendingAssessmentResults.Where(x => x.AssessmentId == assessmentId);
                }

                return new BaseResponse<bool>()
                {
                    Data = pendingAssessmentResults.Any(),
                    StatusCode = StatusCodes.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<bool>()
                {
                    Description = $"[AssessmentService.IsActiveAssessment] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }
    }
}