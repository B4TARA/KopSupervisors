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
        // Преобразование Employee в EmployeeDTO
        public IBaseResponse<EmployeeDTO> CreateEmployeeDTO(Employee employee)
        {
            try
            {
                var dto = new EmployeeDTO
                {
                    Id = employee.Id,
                    FullName = employee.FullName,
                    Position = employee.Position,
                    Subdivision = employee.Subdivision,
                    GradeGroup = employee.GradeGroup,
                    WorkPeriod = employee.WorkPeriod,
                    ContractEndDate = employee.ContractEndDate,
                    ImagePath = employee.ImagePath,
                };

                var lastGrade = employee.Grades.OrderByDescending(x => x.DateOfCreation).FirstOrDefault();

                if (lastGrade == null)
                {
                    dto.LastGrade = null;

                    return new BaseResponse<EmployeeDTO>()
                    {
                        Data = dto,
                        StatusCode = StatusCodes.OK,
                    };
                }

                var gradeDTO = CreateGradeDTO(lastGrade);

                if (gradeDTO.StatusCode != StatusCodes.OK || gradeDTO.Data == null)
                {
                    return new BaseResponse<EmployeeDTO>()
                    {
                        Description = gradeDTO.Description,
                        StatusCode = gradeDTO.StatusCode,
                    };
                }

                dto.LastGrade = gradeDTO.Data;

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
                    Description = $"[MappingService.CreateEmployeeDTO] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        // Преобразование Grade в GradeDTO
        public IBaseResponse<GradeDTO> CreateGradeDTO(Grade grade)
        {
            try
            {
                var dto = new GradeDTO()
                {
                    Id = grade.Id,
                    Number = grade.Number,
                    StartDate = grade.StartDate,
                    EndDate = grade.EndDate,
                    NextGradeDate = grade.NextGradeDate,
                    StrategicTasksConclusion = grade.StrategicTasksConclusion,
                    KPIsConclusion = grade.KPIsConclusion,
                    QualificationConclusion = grade.QualificationConclusion,
                    ManagmentCompetenciesConclusion = grade.ManagmentCompetenciesConclusion,
                    CorporateCompetenciesConclusion = grade.CorporateCompetenciesConclusion,
                };

                if (grade.Qualification != null)
                {
                    var qualification = CreateQualificationDTO(grade.Qualification);

                    if (qualification.StatusCode != StatusCodes.OK || qualification.Data == null)
                    {
                        return new BaseResponse<GradeDTO>()
                        {
                            Description = qualification.Description,
                            StatusCode = qualification.StatusCode,
                        };
                    }

                    dto.Qualification = qualification.Data;
                }

                if (grade.ValueJudgment != null)
                {
                    var valueJudgment = CreateValueJudgmentDTO(grade.ValueJudgment);

                    if (valueJudgment.StatusCode != StatusCodes.OK || valueJudgment.Data == null)
                    {
                        return new BaseResponse<GradeDTO>()
                        {
                            Description = valueJudgment.Description,
                            StatusCode = valueJudgment.StatusCode,
                        };
                    }

                    dto.ValueJudgment = valueJudgment.Data;
                }

                foreach (var markType in grade.Marks.GroupBy(x => x.MarkType).Where(x => x.Key != null))
                {
                    var markTypeDTO = new MarkTypeDTO
                    {
                        Id = markType.Key.Id,
                        Name = markType.Key.Name,
                        Description = markType.Key.Description,
                    };

                    foreach (var mark in markType)
                    {
                        var markDTO = CreateMarkDTO(mark);

                        if (markDTO.StatusCode != StatusCodes.OK || markDTO.Data == null)
                        {
                            return new BaseResponse<GradeDTO>()
                            {
                                Description = markDTO.Description,
                                StatusCode = markDTO.StatusCode,
                            };
                        }

                        markTypeDTO.Marks.Add(markDTO.Data);
                    }

                    dto.MarkTypes.Add(markTypeDTO);
                }

                foreach (var kpi in grade.Kpis)
                {
                    var kpiDTO = CreateKpiDTO(kpi);

                    if (kpiDTO.StatusCode != StatusCodes.OK || kpiDTO.Data == null)
                    {
                        return new BaseResponse<GradeDTO>()
                        {
                            Description = kpiDTO.Description,
                            StatusCode = kpiDTO.StatusCode,
                        };
                    }

                    dto.Kpis.Add(kpiDTO.Data);
                }

                foreach (var project in grade.Projects)
                {
                    var projectDTO = CreateProjectDTO(project);

                    if (projectDTO.StatusCode != StatusCodes.OK || projectDTO.Data == null)
                    {
                        return new BaseResponse<GradeDTO>()
                        {
                            Description = projectDTO.Description,
                            StatusCode = projectDTO.StatusCode,
                        };
                    }

                    dto.Projects.Add(projectDTO.Data);
                }

                foreach (var strategicTask in grade.StrategicTasks)
                {
                    var strategicTaskDTO = CreateStrategicTaskDTO(strategicTask);

                    if (strategicTaskDTO.StatusCode != StatusCodes.OK || strategicTaskDTO.Data == null)
                    {
                        return new BaseResponse<GradeDTO>()
                        {
                            Description = strategicTaskDTO.Description,
                            StatusCode = strategicTaskDTO.StatusCode,
                        };
                    }

                    dto.StrategicTasks.Add(strategicTaskDTO.Data);
                }

                foreach (var trainingEvent in grade.TrainingEvents)
                {
                    var trainingEventDTO = CreateTrainingEventDTO(trainingEvent);

                    if (trainingEventDTO.StatusCode != StatusCodes.OK || trainingEventDTO.Data == null)
                    {
                        return new BaseResponse<GradeDTO>()
                        {
                            Description = trainingEventDTO.Description,
                            StatusCode = trainingEventDTO.StatusCode,
                        };
                    }

                    dto.TrainingEvents.Add(trainingEventDTO.Data);
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
                    Description = $"[MappingService.CreateGradeDTO] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        // Преобразование Qualification в QualificationDTO
        public IBaseResponse<QualificationDTO> CreateQualificationDTO(Qualification qualification)
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
                    StartDate = qualification.StartDate,
                    EndDate = qualification.EndDate,
                    AdditionalEducation = qualification.AdditionalEducation,
                    CurrentDate = qualification.CurrentDate,
                    ExperienceMonths = qualification.ExperienceMonths,
                    ExperienceYears = qualification.ExperienceYears,
                    PreviousPosition1 = qualification.PreviousPosition1,
                    PreviousPosition2 = qualification.PreviousPosition2,
                    CurrentPosition = qualification.CurrentPosition,
                    EmploymentContarctTerminations = qualification.EmploymentContarctTerminations,
                };

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
                    Description = $"[MappingService.CreateQualificationDTO] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        // Преобразование Mark в MarkDTO
        public IBaseResponse<MarkDTO> CreateMarkDTO(Mark mark)
        {
            try
            {
                if (mark.MarkType == null)
                {
                    return new BaseResponse<MarkDTO>()
                    {
                        Description = $"[MappingService.CreateMarkDTO] : Mark.MarkType is null",
                        StatusCode = StatusCodes.EntityNotFound,
                    };
                }

                var dto = new MarkDTO()
                {
                    Id = mark.Id,
                    PercentageValue = mark.PercentageValue,
                    Period = mark.Period,
                    EmployeeId = mark.EmployeeId,
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
                    Description = $"[MappingService.CreateMarkDTO] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        // Преобразование Kpi в KpiDTO
        public IBaseResponse<KpiDTO> CreateKpiDTO(Kpi kpi)
        {
            try
            {
                var dto = new KpiDTO()
                {
                    Id = kpi.Id,
                    Name = kpi.Name,
                    PeriodStartDate = kpi.PeriodStartDate,
                    PeriodEndDate = kpi.PeriodEndDate,
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
                    Description = $"[MappingService.CreateKpiDTO] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        // Преобразование Project в ProjectDTO
        public IBaseResponse<ProjectDTO> CreateProjectDTO(Project project)
        {
            try
            {
                var dto = new ProjectDTO()
                {
                    Id = project.Id,
                    Name = project.Name,
                    SupervisorSspName = project.SupervisorSspName,
                    Stage = project.Stage,
                    StartDate = project.StartDate,
                    EndDate = project.EndDate,
                    CurrentDate = project.CurrentDate,
                    CurrentMonth = project.CurrentMonth,
                    CurrentYear = project.CurrentYear,
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
                    Description = $"[MappingService.CreateProjectDTO] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        // Преобразование StrategicTask в StrategicTaskDTO
        public IBaseResponse<StrategicTaskDTO> CreateStrategicTaskDTO(StrategicTask strategicTask)
        {
            try
            {
                var dto = new StrategicTaskDTO()
                {
                    Id = strategicTask.Id,
                    Name = strategicTask.Name,
                    Purpose = strategicTask.Purpose,
                    PlanDate = strategicTask.PlanDate,
                    FactDate = strategicTask.FactDate,
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
                    Description = $"[MappingService.CreateStrategicTaskDTO] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        // Преобразование ValueJudgment в ValueJudgmentDTO
        public IBaseResponse<ValueJudgmentDTO> CreateValueJudgmentDTO(ValueJudgment valueJudgment)
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
                    Description = $"[MappingService.CreateValueJudgmentDTO] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        // Преобразование TrainingEvent в TrainingEventDTO
        public IBaseResponse<TrainingEventDTO> CreateTrainingEventDTO(TrainingEvent trainingEvent)
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
                    Description = $"[MappingService.CreateTrainingEventDTO] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        // Преобразование Assessment в AssessmentDTO
        public IBaseResponse<AssessmentDTO> CreateAssessmentDTO(Assessment assessment)
        {
            try
            {
                if (assessment.AssessmentType == null)
                {
                    return new BaseResponse<AssessmentDTO>()
                    {
                        Description = $"[MappingService.CreateAssessmentDTO] : Assessment.AssessmentType is null",
                        StatusCode = StatusCodes.EntityNotFound,
                    };
                }
                else if (assessment.AssessmentType.AssessmentMatrix == null)
                {
                    return new BaseResponse<AssessmentDTO>()
                    {
                        Description = $"[MappingService.CreateAssessmentDTO] : Assessment.AssessmentType.AssessmentMatrix is null",
                        StatusCode = StatusCodes.EntityNotFound,
                    };
                }

                var dto = new AssessmentDTO()
                {
                    Id = assessment.Id,
                    Number = assessment.Number,
                    EmployeeId = assessment.EmployeeId,
                    SystemStatus = assessment.SystemStatus,
                    NextAssessmentDate = assessment.NextAssessmentDate,
                };

                foreach (var result in assessment.AssessmentResults)
                {
                    var resultDTO = CreateAssessmentResultDTO(result, assessment.AssessmentType.AssessmentMatrix);

                    if (resultDTO.StatusCode != StatusCodes.OK || resultDTO.Data == null)
                    {
                        return new BaseResponse<AssessmentDTO>()
                        {
                            Description = resultDTO.Description,
                            StatusCode = resultDTO.StatusCode,
                        };
                    }

                    dto.AssessmentResults.Add(resultDTO.Data);
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
                    Description = $"[MappingService.CreateAssessmentDTO] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        // Преобразование AssessmentResult в AssessmentResultDTO
        public IBaseResponse<AssessmentResultDTO> CreateAssessmentResultDTO(AssessmentResult result, AssessmentMatrix matrix)
        {
            try
            {
                if (result.Judge == null)
                {
                    return new BaseResponse<AssessmentResultDTO>()
                    {
                        Description = $"[MappingService.CreateAssessmentResultDTO] : AssessmentResult.Judge is null",
                        StatusCode = StatusCodes.EntityNotFound,
                    };
                }
                else if (result.Judged == null)
                {
                    return new BaseResponse<AssessmentResultDTO>()
                    {
                        Description = $"[MappingService.CreateAssessmentResultDTO] : AssessmentResult.Judged is null",
                        StatusCode = StatusCodes.EntityNotFound,
                    };
                }

                var dto = new AssessmentResultDTO()
                {
                    Id = result.Id,
                    SystemStatus = result.SystemStatus,
                    Sum = result.AssessmentResultValues.Sum(x => x.Value),
                };

                var judgeDTO = CreateEmployeeDTO(result.Judge);

                if (judgeDTO.StatusCode != StatusCodes.OK || judgeDTO.Data == null)
                {
                    return new BaseResponse<AssessmentResultDTO>()
                    {
                        Description = judgeDTO.Description,
                        StatusCode = judgeDTO.StatusCode,
                    };
                }

                dto.Judge = judgeDTO.Data;

                var judgedDTO = CreateEmployeeDTO(result.Judged);

                if (judgedDTO.StatusCode != StatusCodes.OK || judgedDTO.Data == null)
                {
                    return new BaseResponse<AssessmentResultDTO>()
                    {
                        Description = judgedDTO.Description,
                        StatusCode = judgedDTO.StatusCode,
                    };
                }

                dto.Judged = judgedDTO.Data;

                foreach (var value in result.AssessmentResultValues)
                {
                    var valueDTO = CreateAssessmentResultValueDTO(value);

                    if (valueDTO.StatusCode != StatusCodes.OK || valueDTO.Data == null)
                    {
                        return new BaseResponse<AssessmentResultDTO>()
                        {
                            Description = valueDTO.Description,
                            StatusCode = valueDTO.StatusCode,
                        };
                    }

                    dto.Values.Add(valueDTO.Data);
                }

                foreach (var element in matrix.Elements)
                {
                    var elementDTO = CreateAssessmentMatrixElementDTO(element);

                    if (elementDTO.StatusCode != StatusCodes.OK || elementDTO.Data == null)
                    {
                        return new BaseResponse<AssessmentResultDTO>()
                        {
                            Description = elementDTO.Description,
                            StatusCode = elementDTO.StatusCode,
                        };
                    }

                    dto.Elements.Add(elementDTO.Data);
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
                    Description = $"[MappingService.CreateAssessmentResultDTO] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        // Преобразование AssessmentResultValue в AssessmentResultValueDTO
        public IBaseResponse<AssessmentResultValueDTO> CreateAssessmentResultValueDTO(AssessmentResultValue value)
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
                    Description = $"[MappingService.CreateAssessmentResultValueDTO] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        // Преобразование AssessmentMatrixElement в AssessmentMatrixElementDTO
        public IBaseResponse<AssessmentMatrixElementDTO> CreateAssessmentMatrixElementDTO(AssessmentMatrixElement element)
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
                    Description = $"[MappingService.CreateAssessmentMatrixElementDTO] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }
    }
}