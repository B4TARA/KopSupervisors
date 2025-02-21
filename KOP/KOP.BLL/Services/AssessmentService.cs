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
        private readonly ISupervisorService _supervisorService;

        public AssessmentService(IUnitOfWork unitOfWork, IMappingService mappingService, ISupervisorService supervisorService)
        {
            _unitOfWork = unitOfWork;
            _mappingService = mappingService;
            _supervisorService = supervisorService;
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
                                Column = assessmentMatrixElement.Column,
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
                    return new BaseResponse<AssessmentSummaryDto>
                    {
                        Description = $"Оценка с id = {assessmentId} не найдена",
                        StatusCode = StatusCodes.EntityNotFound,
                    };
                }

                var supervisor = await _supervisorService.GetSupervisorForUser(assessment.UserId);
                var assessmentSummaryDto = new AssessmentSummaryDto
                {
                    RowsWithElements = assessment.AssessmentType.AssessmentMatrix.Elements
                        .Select(element => new AssessmentMatrixElementDto
                        {
                            Column = element.Column,
                            Row = element.Row,
                            Value = element.Value,
                            HtmlClassName = element.HtmlClassName,
                        })
                        .GroupBy(x => x.Row)
                        .OrderBy(x => x.Key)
                        .ToList()
                };

                var selfAssessmentResult = assessment.AssessmentResults.FirstOrDefault(x => x.JudgeId == assessment.UserId);
                var supervisorAssessmentResult = assessment.AssessmentResults.FirstOrDefault(x => x.JudgeId == supervisor?.Id);
                var colleaguesAssessmentResults = assessment.AssessmentResults
                    .Where(x => x.JudgeId != supervisor?.Id && x.JudgeId != assessment.UserId)
                    .ToList();

                // Обработка результатов самооценки
                assessmentSummaryDto.SelfAssessmentResultValues = GetAssessmentResultValues(selfAssessmentResult);
                assessmentSummaryDto.AverageSelfValue = CalculateAverage(assessmentSummaryDto.SelfAssessmentResultValues);

                // Обработка результатов оценки руководителя
                assessmentSummaryDto.SupervisorAssessmentResultValues = GetAssessmentResultValues(supervisorAssessmentResult);
                assessmentSummaryDto.AverageSupervisorValue = CalculateAverage(assessmentSummaryDto.SupervisorAssessmentResultValues);

                // Определение типа оценки
                var assessmentType = assessment.AssessmentType?.SystemAssessmentType;
                var completedAssessmentResults = assessment.AssessmentResults
                    .Where(x => x.SystemStatus == SystemStatuses.COMPLETED)
                    .ToList();

                // Установка финализированного статуса
                assessmentSummaryDto.IsFinalized = IsAssessmentFinalized(assessmentType, completedAssessmentResults, assessment.UserId);

                // Обработка результатов коллег
                ProcessColleaguesResults(colleaguesAssessmentResults, assessmentSummaryDto);

                // Обработка результатов завершенных оценок
                ProcessCompletedResults(completedAssessmentResults, assessmentSummaryDto);

                // Обработка интерпретаций
                ProcessInterpretations(assessment, assessmentSummaryDto);

                return new BaseResponse<AssessmentSummaryDto>
                {
                    Data = assessmentSummaryDto,
                    StatusCode = StatusCodes.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<AssessmentSummaryDto>
                {
                    Description = $"[AssessmentService.GetAssessmentSummary] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        private List<AssessmentResultValueDto> GetAssessmentResultValues(AssessmentResult result)
        {
            return result?.AssessmentResultValues?.Select(value => new AssessmentResultValueDto
            {
                Value = value.Value,
                AssessmentMatrixRow = value.AssessmentMatrixRow,
            }).ToList() ?? new List<AssessmentResultValueDto>();
        }

        private double CalculateAverage(List<AssessmentResultValueDto> values)
        {
            return values.Count > 0 ? values.Average(x => x.Value) : 0;
        }

        private bool IsAssessmentFinalized(SystemAssessmentTypes? assessmentType, List<AssessmentResult> completedResults, int userId)
        {
            if (assessmentType == SystemAssessmentTypes.СorporateСompetencies)
            {
                return completedResults.Count == 6;
            }
            else if (assessmentType == SystemAssessmentTypes.ManagementCompetencies)
            {
                return completedResults.Count == 2;
            }
            return false;
        }

        private void ProcessColleaguesResults(List<AssessmentResult> completedColleaguesResults, AssessmentSummaryDto assessmentSummaryDto)
        {
            if (completedColleaguesResults.Any())
            {
                foreach (var result in completedColleaguesResults)
                {
                    foreach (var value in result.AssessmentResultValues)
                    {
                        var avgResult = assessmentSummaryDto.ColleaguesAssessmentResultValues
                            .FirstOrDefault(x => x.AssessmentMatrixRow == value.AssessmentMatrixRow);

                        if (avgResult != null)
                        {
                            avgResult.Value += value.Value;
                            assessmentSummaryDto.ColleaguesSumResult += value.Value;
                        }
                        else
                        {
                            assessmentSummaryDto.ColleaguesAssessmentResultValues.Add(new AssessmentResultValueDto
                            {
                                Value = value.Value,
                                AssessmentMatrixRow = value.AssessmentMatrixRow
                            });
                            assessmentSummaryDto.ColleaguesSumResult += value.Value;
                        }
                    }
                }

                // Вычисление среднего значения для коллег
                foreach (var value in assessmentSummaryDto.ColleaguesAssessmentResultValues)
                {
                    var average = Math.Round(value.Value / completedColleaguesResults.Count, 1);
                    value.Value = average;
                    assessmentSummaryDto.AverageColleaguesResult += average;
                }
            }
        }

        private void ProcessCompletedResults(List<AssessmentResult> completedResults, AssessmentSummaryDto assessmentSummaryDto)
        {
            if (completedResults.Any())
            {
                foreach (var result in completedResults)
                {
                    foreach (var value in result.AssessmentResultValues)
                    {
                        var avgResult = assessmentSummaryDto.AverageValuesByRow
                            .FirstOrDefault(x => x.AssessmentMatrixRow == value.AssessmentMatrixRow);

                        if (avgResult != null)
                        {
                            avgResult.Value += value.Value;
                            assessmentSummaryDto.SumResult += value.Value;
                        }
                        else
                        {
                            assessmentSummaryDto.AverageValuesByRow.Add(new AssessmentResultValueDto
                            {
                                Value = value.Value,
                                AssessmentMatrixRow = value.AssessmentMatrixRow
                            });
                            assessmentSummaryDto.SumResult += value.Value;
                        }
                    }
                }

                // Вычисление среднего значения для завершенных оценок
                foreach (var value in assessmentSummaryDto.AverageValuesByRow)
                {
                    var average = Math.Round(value.Value / completedResults.Count, 1);
                    value.Value = average;
                    assessmentSummaryDto.GeneralAverageResult += average;
                }
            }
        }

        private void ProcessInterpretations(Assessment assessment, AssessmentSummaryDto assessmentSummaryDto)
        {
            foreach (var interpretation in assessment.AssessmentType?.AssessmentInterpretations ?? Enumerable.Empty<AssessmentInterpretation>())
            {
                var createAssessmentInterpretationDtoRes = _mappingService.CreateAssessmentInterpretationDto(interpretation);

                if (!createAssessmentInterpretationDtoRes.HasData)
                {
                    throw new Exception(createAssessmentInterpretationDtoRes.Description);
                }

                var interpretationDto = createAssessmentInterpretationDtoRes.Data;

                if (assessmentSummaryDto.GeneralAverageResult >= interpretationDto.MinValue && assessmentSummaryDto.GeneralAverageResult <= interpretationDto.MaxValue)
                {
                    assessmentSummaryDto.AverageAssessmentInterpretation = interpretationDto;
                }

                assessmentSummaryDto.AssessmentTypeInterpretations.Add(interpretationDto);
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

        // !!! КОСТЫЛЬ-МЕТОД ДЛЯ ВРЕМЕННОЙ ЗАГЛУШКИ !!!
        // Добавить таблицу AssessmentColumns для указания соответствия между столбцом матрицы и диапазоном значений //
        public int GetInterpretationColumnByAssessmentValue(int? value)
        {
            if (value is null)
            {
                return 0;
            }
            else if (1 <= value && value <= 5)
            {
                return 2;
            }
            else if (6 <= value && value <= 8)
            {
                return 3;
            }
            else if (9 <= value && value <= 11)
            {
                return 4;
            }
            else if (12 <= value && value <= 13)
            {
                return 5;
            }

            return 0;
        }
    }
}