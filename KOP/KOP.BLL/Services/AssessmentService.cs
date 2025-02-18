using KOP.BLL.Interfaces;
using KOP.Common.Dtos;
using KOP.Common.Dtos.AssessmentDtos;
using KOP.Common.Enums;
using KOP.Common.Interfaces;
using KOP.DAL.Interfaces;
using KOP.DAL.Entities.AssessmentEntities;
using DocumentFormat.OpenXml.Presentation;

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

                        assessmentDto.CompletedAssessmentResults.Add(assessmentResultDto);
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
                        Description = $"Оценка с id = {assessmentId} не найдена",
                        StatusCode = StatusCodes.EntityNotFound,
                    };
                }

                var assessmentSummaryDto = new AssessmentSummaryDto();
                var assessmentMatrixElementsDtos = assessment.AssessmentType?.AssessmentMatrix?.Elements?
                    .Select(element => new AssessmentMatrixElementDto
                    {
                        Row = element.Row,
                        Value = element.Value,
                        HtmlClassName = element.HtmlClassName,
                    })
                    .ToList() ?? new List<AssessmentMatrixElementDto>();

                assessmentSummaryDto.Elements = assessmentMatrixElementsDtos;
                assessmentSummaryDto.ElementsByRow = assessmentSummaryDto.Elements.GroupBy(x => x.Row).OrderBy(x => x.Key).ToList();

                assessmentSummaryDto.AverageAssessmentResultValues = assessment.AssessmentType?.AssessmentMatrix?.Elements?
                    .Select(element => new AssessmentResultValueDto
                    {
                        Value = 0,
                        AssessmentMatrixRow = element.Row,
                    })
                    .ToList() ?? new List<AssessmentResultValueDto>();

                var selfAssessmentResult = assessment.AssessmentResults.FirstOrDefault(x => x.JudgeId == assessment.UserId);
                var supervisorAssessmentResult = assessment.AssessmentResults.FirstOrDefault(x => x.Judge?.SystemRoles.Contains(SystemRoles.Supervisor) ?? false);

                if (selfAssessmentResult?.AssessmentResultValues != null)
                {
                    assessmentSummaryDto.SelfAssessmentResultValues = selfAssessmentResult.AssessmentResultValues
                        .Select(value => new AssessmentResultValueDto
                        {
                            Value = value.Value,
                            AssessmentMatrixRow = value.AssessmentMatrixRow,
                        })
                        .ToList();
                }

                if (supervisorAssessmentResult?.AssessmentResultValues != null)
                {
                    assessmentSummaryDto.SupervisorAssessmentResultValues = supervisorAssessmentResult.AssessmentResultValues
                        .Select(value => new AssessmentResultValueDto
                        {
                            Value = value.Value,
                            AssessmentMatrixRow = value.AssessmentMatrixRow,
                        })
                        .ToList();
                }

                var assessmentType = assessment.AssessmentType?.SystemAssessmentType;
                var completedAssessmentResults = assessment.AssessmentResults.Where(x => x.SystemStatus == SystemStatuses.COMPLETED).ToList();

                if (assessmentType == SystemAssessmentTypes.СorporateСompetencies)
                {
                    assessmentSummaryDto.IsFinalized = completedAssessmentResults.Count == 6;
                    completedAssessmentResults = completedAssessmentResults.Where(x => x.JudgeId != assessment.UserId).ToList();
                }
                else if (assessmentType == SystemAssessmentTypes.ManagementCompetencies)
                {
                    assessmentSummaryDto.IsFinalized = completedAssessmentResults.Count == 2;
                }

                if (completedAssessmentResults.Any())
                {
                    foreach (var result in completedAssessmentResults)
                    {
                        foreach (var value in result.AssessmentResultValues)
                        {
                            var avgResult = assessmentSummaryDto.AverageAssessmentResultValues.FirstOrDefault(x => x.AssessmentMatrixRow == value.AssessmentMatrixRow);
                            if (avgResult != null)
                            {
                                avgResult.Value += value.Value;
                            }

                            assessmentSummaryDto.SumResult += value.Value;
                        }
                    }

                    foreach (var value in assessmentSummaryDto.AverageAssessmentResultValues)
                    {
                        var sum = value.Value;
                        var average = sum / completedAssessmentResults.Count;

                        value.Value = average;
                        assessmentSummaryDto.AverageResult += average;
                    }
                }

                foreach (var interpretation in assessment.AssessmentType?.AssessmentInterpretations ?? Enumerable.Empty<AssessmentInterpretation>())
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

        public async Task<IBaseResponse<object>> DeleteJudgeForAssessment(int judgeId, int assessmentId)
        {
            try
            {
                var assessmentResulToDelete = await _unitOfWork.AssessmentResults.GetAsync(x => x.AssessmentId == assessmentId && x.JudgeId == judgeId);

                if (assessmentResulToDelete == null)
                {
                    return new BaseResponse<object>()
                    {
                        Description = $"Результат оценки для оценщика с id = {judgeId} и качественной оценки с id = {assessmentId} не найден",
                        StatusCode = StatusCodes.EntityNotFound,
                    };
                }
                else if (assessmentResulToDelete.SystemStatus == SystemStatuses.COMPLETED)
                {
                    return new BaseResponse<object>()
                    {
                        Description = $"Результат оценки с id = {assessmentResulToDelete.Id} невозможно удалить, так как уже была выставлена оценка",
                        StatusCode = StatusCodes.EntityNotFound,
                    };
                }

                _unitOfWork.AssessmentResults.Remove(assessmentResulToDelete);
                await _unitOfWork.CommitAsync();

                return new BaseResponse<object>()
                {
                    StatusCode = StatusCodes.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<object>()
                {
                    Description = $"[AssessmentService.DeleteJudgeForAssessment] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        public async Task<IBaseResponse<object>> AddJudgeForAssessment(int judgeId, int assessmentId)
        {
            try
            {
                var assessment = await _unitOfWork.Assessments.GetAsync(x => x.Id == assessmentId);
                var judge = await _unitOfWork.Users.GetAsync(x => x.Id == judgeId);
                var assessmentResulToAdd = await _unitOfWork.AssessmentResults.GetAsync(x => x.AssessmentId == assessmentId && x.JudgeId == judgeId);

                if (assessmentResulToAdd != null)
                {
                    return new BaseResponse<object>()
                    {
                        Description = $"Результат оценки для оценщика с id = {judgeId} и качественной оценки с id = {assessmentId} уже существует",
                        StatusCode = StatusCodes.EntityNotFound,
                    };
                }

                assessmentResulToAdd = new AssessmentResult
                {
                    SystemStatus = SystemStatuses.PENDING,
                    JudgeId = judgeId,
                    AssessmentId = assessmentId,
                };

                await _unitOfWork.AssessmentResults.AddAsync(assessmentResulToAdd);
                await _unitOfWork.CommitAsync();

                return new BaseResponse<object>()
                {
                    StatusCode = StatusCodes.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<object>()
                {
                    Description = $"[AssessmentService.AddJudgeForAssessment] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }
    }
}