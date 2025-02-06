using KOP.BLL.Interfaces;
using KOP.Common.Dtos;
using KOP.Common.Dtos.AssessmentDtos;
using KOP.Common.Enums;
using KOP.Common.Interfaces;
using KOP.DAL.Entities.AssessmentEntities;
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

        public async Task<IBaseResponse<AssessmentDto>> GetAssessment(int id, SystemStatuses? systemStatus = null)
        {
            try
            {
                var assessment = await _unitOfWork.Assessments.GetAsync(x => x.Id == id, includeProperties: new string[]
                {
                    "AssessmentType.AssessmentMatrix.Elements",
                    "AssessmentResults.AssessmentResultValues",
                    "AssessmentResults.Judge",
                    "AssessmentResults.Judged"
                });

                if (assessment == null)
                {
                    return new BaseResponse<AssessmentDto>()
                    {
                        Description = $"[AssessmentService.GetAssessment] : Качественная оценка с id = {id} не найдена",
                        StatusCode = StatusCodes.EntityNotFound,
                    };
                }

                var assessmentdto = new AssessmentDto
                {
                    Id = id,
                    Number = assessment.Number,
                    UserId = assessment.UserId,
                    SystemStatus = assessment.SystemStatus,
                };

                var assessmentResults = new List<AssessmentResult>();

                if (systemStatus == null)
                {
                    assessmentResults = assessment.AssessmentResults;
                }
                else
                {
                    assessmentResults = assessment.AssessmentResults.Where(x => x.SystemStatus == systemStatus).ToList();
                }

                foreach (var assessmentResult in assessmentResults)
                {
                    var assessmentResultDto = _mappingService.CreateAssessmentResultDto(assessmentResult, assessment.AssessmentType.AssessmentMatrix);

                    if (assessmentResultDto.StatusCode != StatusCodes.OK || assessmentResultDto.Data == null)
                    {
                        return new BaseResponse<AssessmentDto>()
                        {
                            Description = assessmentResultDto.Description,
                            StatusCode = assessmentResultDto.StatusCode,
                        };
                    }

                    assessmentdto.AssessmentResults.Add(assessmentResultDto.Data);
                    assessmentdto.AverageValue += assessmentResultDto.Data.Sum;
                }

                if (assessmentdto.AssessmentResults.Any())
                {
                    assessmentdto.AverageValue = assessmentdto.AverageValue / assessmentdto.AssessmentResults.Count();
                }

                return new BaseResponse<AssessmentDto>()
                {
                    Data = assessmentdto,
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
                var assessmentResult = await _unitOfWork.AssessmentResults.GetAsync(
                        x => x.AssessmentId == assessmentId && x.JudgeId == judgeId,
                        includeProperties: new string[]
                        {
                            "Judge",
                            "Judged",
                            "AssessmentResultValues",
                            "Assessment.AssessmentType.AssessmentMatrix.Elements"
                        });

                if (assessmentResult == null)
                {
                    return new BaseResponse<AssessmentResultDto>()
                    {
                        Description = $"[AssessmentService.GetAssessmentResult] : Результат качественной оценки не найден",
                        StatusCode = StatusCodes.EntityNotFound,
                    };
                }

                var dto = _mappingService.CreateAssessmentResultDto(assessmentResult, assessmentResult.Assessment.AssessmentType.AssessmentMatrix);

                if (dto.StatusCode != StatusCodes.OK || dto.Data == null)
                {
                    return new BaseResponse<AssessmentResultDto>()
                    {
                        Description = dto.Description,
                        StatusCode = dto.StatusCode,
                    };
                }

                return new BaseResponse<AssessmentResultDto>()
                {
                    Data = dto.Data,
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
                var assessmentType = await _unitOfWork.AssessmentTypes.GetAsync(
                        x => x.Id == assessmentTypeId,
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

                var assessments = await _unitOfWork.Assessments.GetAllAsync(
                        x => x.AssessmentTypeId == assessmentTypeId && x.UserId == userId,
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
                            });
                        }

                        assessmentResultDto.ElementsByRow = assessmentResultDto.Elements.GroupBy(x => x.Row).ToList();

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

        public async Task<IBaseResponse<List<AssessmentTypeDto>>> GetUserAssessmentTypes(int userId)
        {
            try
            {
                var userAssessments = await _unitOfWork.Assessments.GetAllAsync(x => x.UserId == userId, includeProperties: new string[]
                {
                    "AssessmentType",
                });

                var assessmentTypes = userAssessments.GroupBy(x => x.AssessmentType);
                var assessmentTypesDtos = new List<AssessmentTypeDto>();

                foreach (var assessmentType in assessmentTypes)
                {
                    var assessmentTypeDto = new AssessmentTypeDto
                    {
                        Id = assessmentType.Key.Id,
                        Name = assessmentType.Key.Name,
                        UserId = userId,
                    };

                    assessmentTypesDtos.Add(assessmentTypeDto);
                }

                return new BaseResponse<List<AssessmentTypeDto>>()
                {
                    Data = assessmentTypesDtos,
                    StatusCode = StatusCodes.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<List<AssessmentTypeDto>>()
                {
                    Description = $"[AssessmentService.GetAssessmentTypes] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        public async Task<IBaseResponse<bool>> IsActiveAssessment(int judgeId, int judgedId, int? assessmentId)
        {
            try
            {
                var pendingAssessmentResults = await _unitOfWork.AssessmentResults.GetAllAsync(x => x.JudgeId == judgeId && x.JudgedId == judgedId && x.SystemStatus == SystemStatuses.PENDING);

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