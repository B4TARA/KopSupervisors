using KOP.BLL.Interfaces;
using KOP.Common.DTOs;
using KOP.Common.DTOs.AssessmentDTOs;
using KOP.Common.DTOs.GradeDTOs;
using KOP.Common.Enums;
using KOP.Common.Interfaces;
using KOP.DAL.Entities;
using KOP.DAL.Entities.AssessmentEntities;
using KOP.DAL.Entities.GradeEntities;

namespace KOP.BLL.Services
{
    public class MappingService : IMappingService
    {
        public IBaseResponse<EmployeeDTO> CreateUserDto(User user)
        {
            try
            {
                var dto = new EmployeeDTO
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Position = user.Position,
                    SubdivisionFromFile = user.SubdivisionFromFile,
                    GradeGroup = user.GradeGroup,
                    WorkPeriod = user.GetWorkPeriod,
                    ContractEndDate = user.ContractEndDate,
                    ImagePath = user.ImagePath,
                };

                var lastGrade = user.Grades.OrderByDescending(x => x.DateOfCreation).FirstOrDefault();

                if (lastGrade == null)
                {
                    dto.LastGrade = null;

                    return new BaseResponse<EmployeeDTO>()
                    {
                        Data = dto,
                        StatusCode = StatusCodes.OK,
                    };
                }

                var gradeDto = CreateGradeDto(lastGrade);

                if (gradeDto.StatusCode != StatusCodes.OK || gradeDto.Data == null)
                {
                    return new BaseResponse<EmployeeDTO>()
                    {
                        Description = gradeDto.Description,
                        StatusCode = gradeDto.StatusCode,
                    };
                }

                dto.LastGrade = gradeDto.Data;

                return new BaseResponse<EmployeeDTO>()
                {
                    Data = dto,
                    StatusCode = StatusCodes.OK,
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<EmployeeDTO>()
                {
                    Description = $"[MappingService.CreateUserDto] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        public IBaseResponse<GradeDTO> CreateGradeDto(Grade grade)
        {
            try
            {
                var dto = new GradeDTO()
                {
                    Id = grade.Id,
                    Number = grade.Number,
                    StartDate = grade.StartDate,
                    EndDate = grade.EndDate,
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
                        return new BaseResponse<GradeDTO>()
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
                        return new BaseResponse<GradeDTO>()
                        {
                            Description = valueJudgmentDto.Description,
                            StatusCode = valueJudgmentDto.StatusCode,
                        };
                    }

                    dto.ValueJudgment = valueJudgmentDto.Data;
                }

                foreach (var markType in grade.Marks.GroupBy(x => x.MarkType).Where(x => x.Key != null))
                {
                    var markTypeDto = new MarkTypeDTO
                    {
                        Id = markType.Key.Id,
                        Name = markType.Key.Name,
                        Description = markType.Key.Description,
                    };

                    foreach (var mark in markType)
                    {
                        var markDto = CreateMarkDto(mark);

                        if (markDto.StatusCode != StatusCodes.OK || markDto.Data == null)
                        {
                            return new BaseResponse<GradeDTO>()
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
                        return new BaseResponse<GradeDTO>()
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
                        return new BaseResponse<GradeDTO>()
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
                        return new BaseResponse<GradeDTO>()
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
                        return new BaseResponse<GradeDTO>()
                        {
                            Description = trainingEventDto.Description,
                            StatusCode = trainingEventDto.StatusCode,
                        };
                    }

                    dto.TrainingEvents.Add(trainingEventDto.Data);
                }

                return new BaseResponse<GradeDTO>()
                {
                    Data = dto,
                    StatusCode = StatusCodes.OK,
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<GradeDTO>()
                {
                    Description = $"[MappingService.CreateGradeDto] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        public IBaseResponse<QualificationDTO> CreateQualificationDto(Qualification qualification)
        {
            try
            {
                var dto = new QualificationDTO()
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
                        return new BaseResponse<QualificationDTO>()
                        {
                            Description = previousJobDto.Description,
                            StatusCode = previousJobDto.StatusCode,
                        };
                    }

                    dto.PreviousJobs.Add(previousJobDto.Data);
                }

                return new BaseResponse<QualificationDTO>()
                {
                    Data = dto,
                    StatusCode = StatusCodes.OK,
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<QualificationDTO>()
                {
                    Description = $"[MappingService.CreateQualificationDto] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        public IBaseResponse<MarkDTO> CreateMarkDto(Mark mark)
        {
            try
            {
                if (mark.MarkType == null)
                {
                    return new BaseResponse<MarkDTO>()
                    {
                        Description = $"[MappingService.CreateMarkDto] : Mark.MarkType is null",
                        StatusCode = StatusCodes.EntityNotFound,
                    };
                }

                var dto = new MarkDTO()
                {
                    Id = mark.Id,
                    PercentageValue = mark.PercentageValue,
                    Period = mark.Period,
                };

                return new BaseResponse<MarkDTO>()
                {
                    Data = dto,
                    StatusCode = StatusCodes.OK,
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<MarkDTO>()
                {
                    Description = $"[MappingService.CreateMarkDto] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        public IBaseResponse<KpiDTO> CreateKpiDto(Kpi kpi)
        {
            try
            {
                var dto = new KpiDTO()
                {
                    Id = kpi.Id,
                    Name = kpi.Name,
                    PeriodStartDateTime = kpi.PeriodStartDate.ToDateTime(TimeOnly.MinValue),
                    PeriodEndDateTime = kpi.PeriodEndDate.ToDateTime(TimeOnly.MinValue),
                    CompletionPercentage = kpi.CompletionPercentage,
                    CalculationMethod = kpi.CalculationMethod,
                };

                return new BaseResponse<KpiDTO>()
                {
                    Data = dto,
                    StatusCode = StatusCodes.OK,
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<KpiDTO>()
                {
                    Description = $"[MappingService.CreateKpiDto] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        public IBaseResponse<ProjectDTO> CreateProjectDto(Project project)
        {
            try
            {
                var dto = new ProjectDTO()
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

                return new BaseResponse<ProjectDTO>()
                {
                    Data = dto,
                    StatusCode = StatusCodes.OK,
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<ProjectDTO>()
                {
                    Description = $"[MappingService.CreateProjectDto] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        public IBaseResponse<StrategicTaskDTO> CreateStrategicTaskDto(StrategicTask strategicTask)
        {
            try
            {
                var dto = new StrategicTaskDTO()
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

                return new BaseResponse<StrategicTaskDTO>()
                {
                    Data = dto,
                    StatusCode = StatusCodes.OK,
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<StrategicTaskDTO>()
                {
                    Description = $"[MappingService.CreateStrategicTaskDto] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        public IBaseResponse<ValueJudgmentDTO> CreateValueJudgmentDto(ValueJudgment valueJudgment)
        {
            try
            {
                var dto = new ValueJudgmentDTO()
                {
                    Id = valueJudgment.Id,
                    Strengths = valueJudgment.Strengths,
                    BehaviorToCorrect = valueJudgment.BehaviorToCorrect,
                    RecommendationsForDevelopment = valueJudgment.RecommendationsForDevelopment,
                };

                return new BaseResponse<ValueJudgmentDTO>()
                {
                    Data = dto,
                    StatusCode = StatusCodes.OK,
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<ValueJudgmentDTO>()
                {
                    Description = $"[MappingService.CreateValueJudgmentDto] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        public IBaseResponse<PreviousJobDTO> CreatePreviousJobDto(PreviousJob previousJob)
        {
            try
            {
                var dto = new PreviousJobDTO()
                {
                    Id = previousJob.Id,
                    StartDateTime = previousJob.StartDate.ToDateTime(TimeOnly.MinValue),
                    EndDateTime = previousJob.EndDate.ToDateTime(TimeOnly.MinValue),
                    OrganizationName = previousJob.OrganizationName,
                    PositionName = previousJob.PositionName
                };

                return new BaseResponse<PreviousJobDTO>()
                {
                    Data = dto,
                    StatusCode = StatusCodes.OK,
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<PreviousJobDTO>()
                {
                    Description = $"[MappingService.CreatePreviousJobDto] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        public IBaseResponse<TrainingEventDTO> CreateTrainingEventDto(TrainingEvent trainingEvent)
        {
            try
            {
                var dto = new TrainingEventDTO()
                {
                    Id = trainingEvent.Id,
                    Name = trainingEvent.Name,
                    Status = trainingEvent.Status,
                    StartDate = trainingEvent.StartDate,
                    EndDate = trainingEvent.EndDate,
                    Competence = trainingEvent.Competence,
                };

                return new BaseResponse<TrainingEventDTO>()
                {
                    Data = dto,
                    StatusCode = StatusCodes.OK,
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<TrainingEventDTO>()
                {
                    Description = $"[MappingService.CreateTrainingEventDto] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        public IBaseResponse<AssessmentDTO> CreateAssessmentDto(Assessment assessment)
        {
            try
            {
                if (assessment.AssessmentType == null)
                {
                    return new BaseResponse<AssessmentDTO>()
                    {
                        Description = $"[MappingService.CreateAssessmentDto] : Assessment.AssessmentType is null",
                        StatusCode = StatusCodes.EntityNotFound,
                    };
                }
                else if (assessment.AssessmentType.AssessmentMatrix == null)
                {
                    return new BaseResponse<AssessmentDTO>()
                    {
                        Description = $"[MappingService.CreateAssessmentDto] : Assessment.AssessmentType.AssessmentMatrix is null",
                        StatusCode = StatusCodes.EntityNotFound,
                    };
                }

                var dto = new AssessmentDTO()
                {
                    Id = assessment.Id,
                    Number = assessment.Number,
                    UserId = assessment.UserId,
                    SystemStatus = assessment.SystemStatus,
                };

                foreach (var result in assessment.AssessmentResults)
                {
                    var resultDto = CreateAssessmentResultDto(result, assessment.AssessmentType.AssessmentMatrix);

                    if (resultDto.StatusCode != StatusCodes.OK || resultDto.Data == null)
                    {
                        return new BaseResponse<AssessmentDTO>()
                        {
                            Description = resultDto.Description,
                            StatusCode = resultDto.StatusCode,
                        };
                    }

                    dto.AssessmentResults.Add(resultDto.Data);
                }

                return new BaseResponse<AssessmentDTO>()
                {
                    Data = dto,
                    StatusCode = StatusCodes.OK,
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<AssessmentDTO>()
                {
                    Description = $"[MappingService.CreateAssessmentDto] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        public IBaseResponse<AssessmentResultDTO> CreateAssessmentResultDto(AssessmentResult result, AssessmentMatrix matrix)
        {
            try
            {
                if (result.Judge == null)
                {
                    return new BaseResponse<AssessmentResultDTO>()
                    {
                        Description = $"[MappingService.CreateAssessmentResultDto] : AssessmentResult.Judge is null",
                        StatusCode = StatusCodes.EntityNotFound,
                    };
                }
                else if (result.Judged == null)
                {
                    return new BaseResponse<AssessmentResultDTO>()
                    {
                        Description = $"[MappingService.CreateAssessmentResultDto] : AssessmentResult.Judged is null",
                        StatusCode = StatusCodes.EntityNotFound,
                    };
                }

                var dto = new AssessmentResultDTO()
                {
                    Id = result.Id,
                    SystemStatus = result.SystemStatus,
                    Sum = result.AssessmentResultValues.Sum(x => x.Value),
                };

                var judgeDto = CreateUserDto(result.Judge);

                if (judgeDto.StatusCode != StatusCodes.OK || judgeDto.Data == null)
                {
                    return new BaseResponse<AssessmentResultDTO>()
                    {
                        Description = judgeDto.Description,
                        StatusCode = judgeDto.StatusCode,
                    };
                }

                dto.Judge = judgeDto.Data;

                var judgedDto = CreateUserDto(result.Judged);

                if (judgedDto.StatusCode != StatusCodes.OK || judgedDto.Data == null)
                {
                    return new BaseResponse<AssessmentResultDTO>()
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
                        return new BaseResponse<AssessmentResultDTO>()
                        {
                            Description = valueDto.Description,
                            StatusCode = valueDto.StatusCode,
                        };
                    }

                    dto.Values.Add(valueDto.Data);
                }

                foreach (var element in matrix.Elements)
                {
                    var elementDto = CreateAssessmentMatrixElementDto(element);

                    if (elementDto.StatusCode != StatusCodes.OK || elementDto.Data == null)
                    {
                        return new BaseResponse<AssessmentResultDTO>()
                        {
                            Description = elementDto.Description,
                            StatusCode = elementDto.StatusCode,
                        };
                    }

                    dto.Elements.Add(elementDto.Data);
                }

                dto.ElementsByRow = dto.Elements.GroupBy(x => x.Row).ToList();
                dto.MaxValue = matrix.MaxAssessmentMatrixResultValue;
                dto.MinValue = matrix.MinAssessmentMatrixResultValue;

                return new BaseResponse<AssessmentResultDTO>()
                {
                    Data = dto,
                    StatusCode = StatusCodes.OK,
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<AssessmentResultDTO>()
                {
                    Description = $"[MappingService.CreateAssessmentResultDto] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        public IBaseResponse<AssessmentResultValueDTO> CreateAssessmentResultValueDto(AssessmentResultValue value)
        {
            try
            {
                var dto = new AssessmentResultValueDTO()
                {
                    Value = value.Value,
                    AssessmentMatrixRow = value.AssessmentMatrixRow,
                };

                return new BaseResponse<AssessmentResultValueDTO>()
                {
                    Data = dto,
                    StatusCode = StatusCodes.OK,
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<AssessmentResultValueDTO>()
                {
                    Description = $"[MappingService.CreateAssessmentResultValueDto] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        public IBaseResponse<AssessmentMatrixElementDTO> CreateAssessmentMatrixElementDto(AssessmentMatrixElement element)
        {
            try
            {
                var dto = new AssessmentMatrixElementDTO()
                {
                    Row = element.Row,
                    Value = element.Value,
                };

                return new BaseResponse<AssessmentMatrixElementDTO>()
                {
                    Data = dto,
                    StatusCode = StatusCodes.OK,
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<AssessmentMatrixElementDTO>()
                {
                    Description = $"[MappingService.CreateAssessmentMatrixElementDto] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }
    }
}