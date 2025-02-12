using KOP.BLL.Interfaces;
using KOP.Common.Dtos;
using KOP.Common.Dtos.AssessmentDtos;
using KOP.Common.Dtos.GradeDtos;
using KOP.Common.Enums;
using KOP.Common.Interfaces;
using KOP.DAL.Entities;
using KOP.DAL.Entities.AssessmentEntities;
using KOP.DAL.Entities.GradeEntities;

namespace KOP.BLL.Services
{
    public class MappingService : IMappingService
    {
        public IBaseResponse<UserDto> CreateUserDto(User user)
        {
            try
            {
                var dto = new UserDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Position = user.Position,
                    SubdivisionFromFile = user.SubdivisionFromFile,
                    GradeGroup = user.GradeGroup,
                    WorkPeriod = user.GetWorkPeriod,
                    NextGradeStartDate = user.GetNextGradeStartDate,
                    ContractEndDate = user.ContractEndDate,
                    ImagePath = user.ImagePath,
                };

                var lastGrade = user.Grades.OrderByDescending(x => x.DateOfCreation).FirstOrDefault();

                if (lastGrade == null)
                {
                    dto.LastGrade = null;

                    return new BaseResponse<UserDto>()
                    {
                        Data = dto,
                        StatusCode = StatusCodes.OK,
                    };
                }

                var gradeDto = CreateGradeDto(lastGrade, new List<MarkType>());

                if (!gradeDto.HasData)
                {
                    return new BaseResponse<UserDto>()
                    {
                        Description = gradeDto.Description,
                        StatusCode = gradeDto.StatusCode,
                    };
                }

                dto.LastGrade = gradeDto.Data;

                return new BaseResponse<UserDto>()
                {
                    Data = dto,
                    StatusCode = StatusCodes.OK,
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<UserDto>()
                {
                    Description = $"[MappingService.CreateUserDto] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        public IBaseResponse<GradeDto> CreateGradeDto(Grade grade, IEnumerable<MarkType> allMarkTypes)
        {
            try
            {
                var dto = new GradeDto()
                {
                    Id = grade.Id,
                    Number = grade.Number,
                    StartDate = grade.StartDate,
                    EndDate = grade.EndDate,
                    SystemStatus = grade.SystemStatus,
                    IsProjectsFinalized = grade.IsProjectsFinalized,
                    IsStrategicTasksFinalized = grade.IsStrategicTasksFinalized,
                    IsKpisFinalized = grade.IsKpisFinalized,
                    IsMarksFinalized = grade.IsMarksFinalized,
                    IsQualificationFinalized = grade.IsQualificationFinalized,
                    IsValueJudgmentFinalized = grade.IsValueJudgmentFinalized,
                    StrategicTasksConclusion = grade.StrategicTasksConclusion,
                    KPIsConclusion = grade.KPIsConclusion,
                    QualificationConclusion = grade.QualificationConclusion,
                    ManagmentCompetenciesConclusion = grade.ManagmentCompetenciesConclusion,
                    CorporateCompetenciesConclusion = grade.CorporateCompetenciesConclusion,
                };

                if (grade.Qualification != null)
                {
                    var qualificationDto = CreateQualificationDto(grade.Qualification);

                    if (qualificationDto.StatusCode != StatusCodes.OK || qualificationDto.Data == null)
                    {
                        return new BaseResponse<GradeDto>()
                        {
                            Description = qualificationDto.Description,
                            StatusCode = qualificationDto.StatusCode,
                        };
                    }

                    dto.Qualification = qualificationDto.Data;
                }

                if (grade.ValueJudgment != null)
                {
                    var valueJudgmentDto = CreateValueJudgmentDto(grade.ValueJudgment);

                    if (valueJudgmentDto.StatusCode != StatusCodes.OK || valueJudgmentDto.Data == null)
                    {
                        return new BaseResponse<GradeDto>()
                        {
                            Description = valueJudgmentDto.Description,
                            StatusCode = valueJudgmentDto.StatusCode,
                        };
                    }

                    dto.ValueJudgment = valueJudgmentDto.Data;
                }

                foreach (var markType in allMarkTypes)
                {
                    var markTypeDto = new MarkTypeDto
                    {
                        Id = markType.Id,
                        Name = markType.Name,
                        Description = markType.Description,
                    };

                    foreach (var mark in markType.Marks.Where(x => x.GradeId == grade.Id))
                    {
                        var markDto = CreateMarkDto(mark);

                        if (markDto.StatusCode != StatusCodes.OK || markDto.Data == null)
                        {
                            return new BaseResponse<GradeDto>()
                            {
                                Description = markDto.Description,
                                StatusCode = markDto.StatusCode,
                            };
                        }

                        markTypeDto.Marks.Add(markDto.Data);
                    }

                    dto.MarkTypes.Add(markTypeDto);
                }

                foreach (var kpi in grade.Kpis)
                {
                    var kpiDto = CreateKpiDto(kpi);

                    if (kpiDto.StatusCode != StatusCodes.OK || kpiDto.Data == null)
                    {
                        return new BaseResponse<GradeDto>()
                        {
                            Description = kpiDto.Description,
                            StatusCode = kpiDto.StatusCode,
                        };
                    }

                    dto.Kpis.Add(kpiDto.Data);
                }

                foreach (var project in grade.Projects)
                {
                    var projectDto = CreateProjectDto(project);

                    if (projectDto.StatusCode != StatusCodes.OK || projectDto.Data == null)
                    {
                        return new BaseResponse<GradeDto>()
                        {
                            Description = projectDto.Description,
                            StatusCode = projectDto.StatusCode,
                        };
                    }

                    dto.Projects.Add(projectDto.Data);
                }

                foreach (var strategicTask in grade.StrategicTasks)
                {
                    var strategicTaskDto = CreateStrategicTaskDto(strategicTask);

                    if (strategicTaskDto.StatusCode != StatusCodes.OK || strategicTaskDto.Data == null)
                    {
                        return new BaseResponse<GradeDto>()
                        {
                            Description = strategicTaskDto.Description,
                            StatusCode = strategicTaskDto.StatusCode,
                        };
                    }

                    dto.StrategicTasks.Add(strategicTaskDto.Data);
                }

                foreach (var trainingEvent in grade.TrainingEvents)
                {
                    var trainingEventDto = CreateTrainingEventDto(trainingEvent);

                    if (trainingEventDto.StatusCode != StatusCodes.OK || trainingEventDto.Data == null)
                    {
                        return new BaseResponse<GradeDto>()
                        {
                            Description = trainingEventDto.Description,
                            StatusCode = trainingEventDto.StatusCode,
                        };
                    }

                    dto.TrainingEvents.Add(trainingEventDto.Data);
                }

                return new BaseResponse<GradeDto>()
                {
                    Data = dto,
                    StatusCode = StatusCodes.OK,
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<GradeDto>()
                {
                    Description = $"[MappingService.CreateGradeDto] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        public IBaseResponse<QualificationDto> CreateQualificationDto(Qualification qualification)
        {
            try
            {
                var dto = new QualificationDto()
                {
                    Id = qualification.Id,
                    SupervisorSspName = qualification.SupervisorSspName,
                    Link = qualification.Link,
                    HigherEducation = qualification.HigherEducation,
                    Speciality = qualification.Speciality,
                    QualificationResult = qualification.QualificationResult,
                    StartDateTime = qualification.StartDate.ToDateTime(TimeOnly.MinValue),
                    EndDateTime = qualification.EndDate.ToDateTime(TimeOnly.MinValue),
                    AdditionalEducation = qualification.AdditionalEducation,
                    CurrentStatusDateTime = qualification.CurrentStatusDate.ToDateTime(TimeOnly.MinValue),
                    CurrentExperienceYears = qualification.CurrentExperienceYears,
                    CurrentExperienceMonths = qualification.CurrentExperienceMonths,
                    CurrentJobStartDateTime = qualification.CurrentJobStartDate.ToDateTime(TimeOnly.MinValue),
                    CurrentJobPositionName = qualification.CurrentJobPositionName,
                    EmploymentContarctTerminations = qualification.EmploymentContarctTerminations,
                };

                foreach (var previousJob in qualification.PreviousJobs)
                {
                    var previousJobDto = CreatePreviousJobDto(previousJob);

                    if (previousJobDto.StatusCode != StatusCodes.OK || previousJobDto.Data == null)
                    {
                        return new BaseResponse<QualificationDto>()
                        {
                            Description = previousJobDto.Description,
                            StatusCode = previousJobDto.StatusCode,
                        };
                    }

                    dto.PreviousJobs.Add(previousJobDto.Data);
                }

                return new BaseResponse<QualificationDto>()
                {
                    Data = dto,
                    StatusCode = StatusCodes.OK,
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<QualificationDto>()
                {
                    Description = $"[MappingService.CreateQualificationDto] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        public IBaseResponse<MarkDto> CreateMarkDto(Mark mark)
        {
            try
            {
                if (mark.MarkType == null)
                {
                    return new BaseResponse<MarkDto>()
                    {
                        Description = $"[MappingService.CreateMarkDto] : Mark.MarkType is null",
                        StatusCode = StatusCodes.EntityNotFound,
                    };
                }

                var dto = new MarkDto()
                {
                    Id = mark.Id,
                    PercentageValue = mark.PercentageValue,
                    Period = mark.Period,
                };

                return new BaseResponse<MarkDto>()
                {
                    Data = dto,
                    StatusCode = StatusCodes.OK,
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<MarkDto>()
                {
                    Description = $"[MappingService.CreateMarkDto] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        public IBaseResponse<KpiDto> CreateKpiDto(Kpi kpi)
        {
            try
            {
                var dto = new KpiDto()
                {
                    Id = kpi.Id,
                    Name = kpi.Name,
                    PeriodStartDateTime = kpi.PeriodStartDate.ToDateTime(TimeOnly.MinValue),
                    PeriodEndDateTime = kpi.PeriodEndDate.ToDateTime(TimeOnly.MinValue),
                    CompletionPercentage = kpi.CompletionPercentage,
                    CalculationMethod = kpi.CalculationMethod,
                };

                return new BaseResponse<KpiDto>()
                {
                    Data = dto,
                    StatusCode = StatusCodes.OK,
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<KpiDto>()
                {
                    Description = $"[MappingService.CreateKpiDto] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        public IBaseResponse<ProjectDto> CreateProjectDto(Project project)
        {
            try
            {
                var dto = new ProjectDto()
                {
                    Id = project.Id,
                    Name = project.Name,
                    SupervisorSspName = project.SupervisorSspName,
                    Stage = project.Stage,
                    StartDateTime = project.StartDate.ToDateTime(TimeOnly.MinValue),
                    EndDateTime = project.EndDate.ToDateTime(TimeOnly.MinValue),
                    CurrentStatusDateTime = project.CurrentStatusDate.ToDateTime(TimeOnly.MinValue),
                    PlanStages = project.PlanStages,
                    FactStages = project.FactStages,
                    SPn = project.SPn,
                };

                return new BaseResponse<ProjectDto>()
                {
                    Data = dto,
                    StatusCode = StatusCodes.OK,
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<ProjectDto>()
                {
                    Description = $"[MappingService.CreateProjectDto] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        public IBaseResponse<StrategicTaskDto> CreateStrategicTaskDto(StrategicTask strategicTask)
        {
            try
            {
                var dto = new StrategicTaskDto()
                {
                    Id = strategicTask.Id,
                    Name = strategicTask.Name,
                    Purpose = strategicTask.Purpose,
                    PlanDateTime = strategicTask.PlanDate.ToDateTime(TimeOnly.MinValue),
                    FactDateTime = strategicTask.FactDate.ToDateTime(TimeOnly.MinValue),
                    PlanResult = strategicTask.PlanResult,
                    FactResult = strategicTask.FactResult,
                    Remark = strategicTask.Remark,
                };

                return new BaseResponse<StrategicTaskDto>()
                {
                    Data = dto,
                    StatusCode = StatusCodes.OK,
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<StrategicTaskDto>()
                {
                    Description = $"[MappingService.CreateStrategicTaskDto] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        public IBaseResponse<ValueJudgmentDto> CreateValueJudgmentDto(ValueJudgment valueJudgment)
        {
            try
            {
                var dto = new ValueJudgmentDto()
                {
                    Id = valueJudgment.Id,
                    Strengths = valueJudgment.Strengths,
                    BehaviorToCorrect = valueJudgment.BehaviorToCorrect,
                    RecommendationsForDevelopment = valueJudgment.RecommendationsForDevelopment,
                };

                return new BaseResponse<ValueJudgmentDto>()
                {
                    Data = dto,
                    StatusCode = StatusCodes.OK,
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<ValueJudgmentDto>()
                {
                    Description = $"[MappingService.CreateValueJudgmentDto] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        public IBaseResponse<PreviousJobDto> CreatePreviousJobDto(PreviousJob previousJob)
        {
            try
            {
                var dto = new PreviousJobDto()
                {
                    Id = previousJob.Id,
                    StartDateTime = previousJob.StartDate.ToDateTime(TimeOnly.MinValue),
                    EndDateTime = previousJob.EndDate.ToDateTime(TimeOnly.MinValue),
                    OrganizationName = previousJob.OrganizationName,
                    PositionName = previousJob.PositionName
                };

                return new BaseResponse<PreviousJobDto>()
                {
                    Data = dto,
                    StatusCode = StatusCodes.OK,
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<PreviousJobDto>()
                {
                    Description = $"[MappingService.CreatePreviousJobDto] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        public IBaseResponse<TrainingEventDto> CreateTrainingEventDto(TrainingEvent trainingEvent)
        {
            try
            {
                var dto = new TrainingEventDto()
                {
                    Id = trainingEvent.Id,
                    Name = trainingEvent.Name,
                    Status = trainingEvent.Status,
                    StartDate = trainingEvent.StartDate,
                    EndDate = trainingEvent.EndDate,
                    Competence = trainingEvent.Competence,
                };

                return new BaseResponse<TrainingEventDto>()
                {
                    Data = dto,
                    StatusCode = StatusCodes.OK,
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<TrainingEventDto>()
                {
                    Description = $"[MappingService.CreateTrainingEventDto] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        public IBaseResponse<AssessmentDto> CreateAssessmentDto(Assessment assessment)
        {
            try
            {
                if (assessment.AssessmentType == null)
                {
                    return new BaseResponse<AssessmentDto>()
                    {
                        Description = $"[MappingService.CreateAssessmentDto] : Assessment.AssessmentType is null",
                        StatusCode = StatusCodes.EntityNotFound,
                    };
                }
                else if (assessment.AssessmentType.AssessmentMatrix == null)
                {
                    return new BaseResponse<AssessmentDto>()
                    {
                        Description = $"[MappingService.CreateAssessmentDto] : Assessment.AssessmentType.AssessmentMatrix is null",
                        StatusCode = StatusCodes.EntityNotFound,
                    };
                }

                var dto = new AssessmentDto()
                {
                    Id = assessment.Id,
                    Number = assessment.Number,
                    UserId = assessment.UserId,
                    SystemStatus = assessment.SystemStatus,
                };

                var completedAssessmentResults = assessment.AssessmentResults.Where(x => x.SystemStatus == SystemStatuses.COMPLETED);

                if (assessment.AssessmentType.SystemAssessmentType == SystemAssessmentTypes.СorporateСompetencies)
                {
                    completedAssessmentResults = completedAssessmentResults.Where(x => x.JudgeId != assessment.UserId);
                }

                foreach (var result in completedAssessmentResults)
                {
                    var resultDto = CreateAssessmentResultDto(result, assessment.AssessmentType);

                    if (!resultDto.HasData)
                    {
                        return new BaseResponse<AssessmentDto>()
                        {
                            Description = resultDto.Description,
                            StatusCode = resultDto.StatusCode,
                        };
                    }

                    dto.AssessmentResults.Add(resultDto.Data);
                    dto.SumValue += resultDto.Data.Sum;
                }

                if (dto.AssessmentResults.Any())
                {
                    dto.AverageValue = dto.SumValue / dto.AssessmentResults.Count();
                }

                foreach (var interpretation in assessment.AssessmentType.AssessmentInterpretations)
                {
                    var createAssessmentInterpretationDtoRes = CreateAssessmentInterpretationDto(interpretation);

                    if (!createAssessmentInterpretationDtoRes.HasData)
                    {
                        return new BaseResponse<AssessmentDto>()
                        {
                            Description = createAssessmentInterpretationDtoRes.Description,
                            StatusCode = createAssessmentInterpretationDtoRes.StatusCode,
                        };
                    }

                    var interpretationDto = createAssessmentInterpretationDtoRes.Data;

                    if (dto.AverageValue >= interpretationDto.MinValue && dto.AverageValue <= interpretationDto.MaxValue)
                    {
                        dto.AverageAssessmentInterpretation = interpretationDto;
                    }    

                    dto.AssessmentTypeInterpretations.Add(interpretationDto);
                }

                return new BaseResponse<AssessmentDto>()
                {
                    Data = dto,
                    StatusCode = StatusCodes.OK,
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<AssessmentDto>()
                {
                    Description = $"[MappingService.CreateAssessmentDto] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        public IBaseResponse<AssessmentResultDto> CreateAssessmentResultDto(AssessmentResult result, AssessmentType type)
        {
            try
            {
                if (result.Judge == null)
                {
                    return new BaseResponse<AssessmentResultDto>()
                    {
                        Description = $"[MappingService.CreateAssessmentResultDto] : AssessmentResult.Judge is null",
                        StatusCode = StatusCodes.EntityNotFound,
                    };
                }

                var dto = new AssessmentResultDto()
                {
                    Id = result.Id,
                    SystemStatus = result.SystemStatus,
                    Sum = result.AssessmentResultValues.Sum(x => x.Value),
                };

                var assessmentInterpretation = type.AssessmentInterpretations.FirstOrDefault(x => x.MinValue <= dto.Sum && x.MaxValue >= dto.Sum);

                if (assessmentInterpretation != null)
                {
                    dto.HtmlClassName = assessmentInterpretation.HtmlClassName;
                }

                var judgeDto = CreateUserDto(result.Judge);

                if (judgeDto.StatusCode != StatusCodes.OK || judgeDto.Data == null)
                {
                    return new BaseResponse<AssessmentResultDto>()
                    {
                        Description = judgeDto.Description,
                        StatusCode = judgeDto.StatusCode,
                    };
                }

                dto.Judge = judgeDto.Data;

                var judgedDto = CreateUserDto(result.Assessment.User);

                if (judgedDto.StatusCode != StatusCodes.OK || judgedDto.Data == null)
                {
                    return new BaseResponse<AssessmentResultDto>()
                    {
                        Description = judgedDto.Description,
                        StatusCode = judgedDto.StatusCode,
                    };
                }

                dto.Judged = judgedDto.Data;

                foreach (var value in result.AssessmentResultValues)
                {
                    var valueDto = CreateAssessmentResultValueDto(value);

                    if (valueDto.StatusCode != StatusCodes.OK || valueDto.Data == null)
                    {
                        return new BaseResponse<AssessmentResultDto>()
                        {
                            Description = valueDto.Description,
                            StatusCode = valueDto.StatusCode,
                        };
                    }

                    dto.Values.Add(valueDto.Data);
                }

                foreach (var element in type.AssessmentMatrix.Elements)
                {
                    var elementDto = CreateAssessmentMatrixElementDto(element);

                    if (elementDto.StatusCode != StatusCodes.OK || elementDto.Data == null)
                    {
                        return new BaseResponse<AssessmentResultDto>()
                        {
                            Description = elementDto.Description,
                            StatusCode = elementDto.StatusCode,
                        };
                    }

                    dto.Elements.Add(elementDto.Data);
                }

                dto.ElementsByRow = dto.Elements.GroupBy(x => x.Row).OrderBy(x => x.Key).ToList();
                dto.MaxValue = type.AssessmentMatrix.MaxAssessmentMatrixResultValue;
                dto.MinValue = type.AssessmentMatrix.MinAssessmentMatrixResultValue;

                return new BaseResponse<AssessmentResultDto>()
                {
                    Data = dto,
                    StatusCode = StatusCodes.OK,
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<AssessmentResultDto>()
                {
                    Description = $"[MappingService.CreateAssessmentResultDto] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        public IBaseResponse<AssessmentInterpretationDto> CreateAssessmentInterpretationDto(AssessmentInterpretation interpretation)
        {
            try
            {
                var dto = new AssessmentInterpretationDto()
                {
                    MinValue = interpretation.MinValue,
                    MaxValue = interpretation.MaxValue,
                    Competence = interpretation.Competence,
                    Level = interpretation.Level,
                    HtmlClassName = interpretation.HtmlClassName,
                };

                return new BaseResponse<AssessmentInterpretationDto>()
                {
                    Data = dto,
                    StatusCode = StatusCodes.OK,
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<AssessmentInterpretationDto>()
                {
                    Description = $"[MappingService.CreateAssessmentInterpretationDto] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        public IBaseResponse<AssessmentResultValueDto> CreateAssessmentResultValueDto(AssessmentResultValue value)
        {
            try
            {
                var dto = new AssessmentResultValueDto()
                {
                    Value = value.Value,
                    AssessmentMatrixRow = value.AssessmentMatrixRow,
                };

                return new BaseResponse<AssessmentResultValueDto>()
                {
                    Data = dto,
                    StatusCode = StatusCodes.OK,
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<AssessmentResultValueDto>()
                {
                    Description = $"[MappingService.CreateAssessmentResultValueDto] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        public IBaseResponse<AssessmentMatrixElementDto> CreateAssessmentMatrixElementDto(AssessmentMatrixElement element)
        {
            try
            {
                var dto = new AssessmentMatrixElementDto()
                {
                    Row = element.Row,
                    Value = element.Value,
                    HtmlClassName = element.HtmlClassName,
                };

                return new BaseResponse<AssessmentMatrixElementDto>()
                {
                    Data = dto,
                    StatusCode = StatusCodes.OK,
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<AssessmentMatrixElementDto>()
                {
                    Description = $"[MappingService.CreateAssessmentMatrixElementDto] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }
    }
}