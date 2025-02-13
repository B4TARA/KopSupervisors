using KOP.BLL.Interfaces;
using KOP.Common.Dtos;
using KOP.Common.Dtos.GradeDtos;
using KOP.Common.Enums;
using KOP.Common.Interfaces;
using KOP.DAL.Entities.GradeEntities;
using KOP.DAL.Interfaces;

namespace KOP.BLL.Services
{
    public class GradeService : IGradeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMappingService _mappingService;

        public GradeService(IUnitOfWork unitOfWork, IMappingService mappingService)
        {
            _unitOfWork = unitOfWork;
            _mappingService = mappingService;
        }

        public async Task<IBaseResponse<GradeDto>> GetGrade(int gradeId, List<GradeEntities> gradeEntitiesList)
        {
            try
            {
                var includeProperties = new List<string>();
                var allMarkTypes = new List<MarkType>();

                foreach (var gradeEntity in gradeEntitiesList)
                {
                    var strGradeEntity = Enum.GetName(typeof(GradeEntities), gradeEntity);

                    if (strGradeEntity is null)
                    {
                        continue;
                    }

                    // УБРАТЬ ЭТОТ КОСТЫЛЬ !!!

                    if (gradeEntity == GradeEntities.Marks)
                    {
                        var allMarkTypesDbRes = await _unitOfWork.MarkTypes.GetAllAsync(includeProperties: "Marks");
                        allMarkTypes = allMarkTypesDbRes.ToList();
                    }
                    // УБРАТЬ ЭТОТ КОСТЫЛЬ !!!
                    else if (gradeEntity == GradeEntities.Qualification)
                    {
                        strGradeEntity += ".PreviousJobs";
                    }

                    includeProperties.Add(strGradeEntity);
                }

                var grade = await _unitOfWork.Grades.GetAsync(x => x.Id == gradeId, includeProperties: includeProperties.ToArray());

                if (grade == null)
                {
                    return new BaseResponse<GradeDto>()
                    {
                        Description = $"[GradeService.GetGrade] : Оценка с id = {gradeId} не найдена",
                        StatusCode = StatusCodes.EntityNotFound,
                    };
                }

                var gradeDto = _mappingService.CreateGradeDto(grade, allMarkTypes);

                if (gradeDto.StatusCode != StatusCodes.OK || gradeDto.Data == null)
                {
                    return new BaseResponse<GradeDto>()
                    {
                        StatusCode = gradeDto.StatusCode,
                        Description = gradeDto.Description
                    };
                }

                return new BaseResponse<GradeDto>()
                {
                    StatusCode = StatusCodes.OK,
                    Data = gradeDto.Data
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<GradeDto>()
                {
                    Description = $"[GradeService.GetGrade] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        public async Task<IBaseResponse<object>> EditGrade(GradeDto dto)
        {
            try
            {
                var grade = await _unitOfWork.Grades.GetAsync(x => x.Id == dto.Id);

                if (grade is null)
                {
                    return new BaseResponse<object>()
                    {
                        Description = $"[GradeService.EditGrade] :Оценка с id = {dto.Id} не найдена",
                        StatusCode = StatusCodes.EntityNotFound
                    };
                }

                var strategicTasks = new List<StrategicTask>();
                var kpis = new List<Kpi>();
                var projects = new List<Project>();
                var marks = new List<Mark>();

                foreach (var strategicTaskDto in dto.StrategicTasks)
                {
                    var strategicTask = new StrategicTask
                    {
                        Name = strategicTaskDto.Name ?? "Название",
                        Purpose = strategicTaskDto.Purpose ?? "Цель",
                        PlanDate = strategicTaskDto.PlanDate,
                        FactDate = strategicTaskDto.FactDate,
                        PlanResult = strategicTaskDto.PlanResult ?? "План",
                        FactResult = strategicTaskDto.FactResult ?? "Факт",
                        Remark = strategicTaskDto.Remark,
                    };

                    strategicTasks.Add(strategicTask);
                }

                foreach (var kpiDto in dto.Kpis)
                {
                    var kpi = new Kpi
                    {
                        Name = kpiDto.Name ?? "КПЭ",
                        PeriodStartDate = kpiDto.PeriodStartDate,
                        PeriodEndDate = kpiDto.PeriodEndDate,
                        CompletionPercentage = kpiDto.CompletionPercentage,
                        CalculationMethod = kpiDto.CalculationMethod ?? "Методика расчета",
                    };

                    kpis.Add(kpi);
                }

                foreach (var projectDto in dto.Projects)
                {
                    var project = new Project
                    {
                        Name = projectDto.Name ?? "ФИО руководителя ССП",
                        SupervisorSspName = projectDto.SupervisorSspName ?? "Наименование проекта",
                        Stage = projectDto.Stage ?? "Этап проекта",
                        StartDate = projectDto.StartDate,
                        EndDate = projectDto.EndDate,
                        CurrentStatusDate = projectDto.CurrentStatusDate,
                        PlanStages = projectDto.PlanStages,
                        FactStages = projectDto.FactStages,
                        SPn = projectDto.SPn,
                    };

                    projects.Add(project);
                }

                foreach (var markTypeDto in dto.MarkTypes)
                {
                    foreach (var markDto in markTypeDto.Marks)
                    {
                        var mark = new Mark
                        {
                            PercentageValue = markDto.PercentageValue,
                            Period = markDto.Period ?? "Период",
                            MarkTypeId = markTypeDto.Id,
                        };

                        marks.Add(mark);
                    }
                }

                if (dto.ValueJudgment is not null)
                {
                    var valueJudgment = await _unitOfWork.ValueJudgments.GetAsync(x => x.Id == dto.ValueJudgment.Id);

                    if (valueJudgment is null)
                    {
                        grade.ValueJudgment = new ValueJudgment
                        {
                            Strengths = dto.ValueJudgment.Strengths ?? "",
                            BehaviorToCorrect = dto.ValueJudgment.BehaviorToCorrect ?? "",
                            RecommendationsForDevelopment = dto.ValueJudgment.RecommendationsForDevelopment ?? "",
                        };
                    }
                    else
                    {
                        valueJudgment.Strengths = dto.ValueJudgment.Strengths;
                        valueJudgment.BehaviorToCorrect = dto.ValueJudgment.BehaviorToCorrect;
                        valueJudgment.RecommendationsForDevelopment = dto.ValueJudgment.RecommendationsForDevelopment;
                    }
                }

                if (dto.Qualification is not null)
                {
                    var qualification = await _unitOfWork.Qualifications.GetAsync(x => x.Id == dto.Qualification.Id);

                    if (qualification is null)
                    {
                        grade.Qualification = new Qualification
                        {
                            SupervisorSspName = dto.Qualification.SupervisorSspName ?? "",
                            Link = dto.Qualification.Link ?? "",
                            HigherEducation = dto.Qualification.HigherEducation ?? "",
                            Speciality = dto.Qualification.Speciality ?? "",
                            QualificationResult = dto.Qualification.QualificationResult ?? "",
                            StartDate = dto.Qualification.StartDate,
                            EndDate = dto.Qualification.EndDate,
                            AdditionalEducation = dto.Qualification.AdditionalEducation ?? "",
                            CurrentStatusDate = dto.Qualification.CurrentStatusDate,
                            CurrentExperienceYears = dto.Qualification.CurrentExperienceYears,
                            CurrentExperienceMonths = dto.Qualification.CurrentExperienceMonths,
                            CurrentJobStartDate = dto.Qualification.CurrentJobStartDate,
                            CurrentJobPositionName = dto.Qualification.CurrentJobPositionName ?? "",
                            EmploymentContarctTerminations = dto.Qualification.EmploymentContarctTerminations ?? "",
                        };

                        foreach (var previousJob in dto.Qualification.PreviousJobs)
                        {
                            grade.Qualification.PreviousJobs.Add(new PreviousJob
                            {
                                StartDate = previousJob.StartDate,
                                EndDate = previousJob.EndDate,
                                OrganizationName = previousJob.OrganizationName ?? "",
                                PositionName = previousJob.PositionName ?? "",
                            });
                        }
                    }
                    else
                    {
                        qualification.SupervisorSspName = dto.Qualification.SupervisorSspName;
                        qualification.Link = dto.Qualification.Link;
                        qualification.HigherEducation = dto.Qualification.HigherEducation;
                        qualification.Speciality = dto.Qualification.Speciality;
                        qualification.QualificationResult = dto.Qualification.QualificationResult;
                        qualification.StartDate = dto.Qualification.StartDate;
                        qualification.EndDate = dto.Qualification.EndDate;
                        qualification.AdditionalEducation = dto.Qualification.AdditionalEducation;
                        qualification.CurrentStatusDate = dto.Qualification.CurrentStatusDate;
                        qualification.CurrentExperienceYears = dto.Qualification.CurrentExperienceYears;
                        qualification.CurrentExperienceMonths = dto.Qualification.CurrentExperienceMonths;
                        qualification.CurrentJobStartDate = dto.Qualification.CurrentJobStartDate;
                        qualification.CurrentJobPositionName = dto.Qualification.CurrentJobPositionName;
                        qualification.EmploymentContarctTerminations = dto.Qualification.EmploymentContarctTerminations;

                        foreach (var previousJob in dto.Qualification.PreviousJobs)
                        {
                            qualification.PreviousJobs.Add(new PreviousJob
                            {
                                StartDate = previousJob.StartDate,
                                EndDate = previousJob.EndDate,
                                OrganizationName = previousJob.OrganizationName ?? "",
                                PositionName = previousJob.PositionName ?? "",
                            });
                        }
                    }

                    grade.QualificationConclusion = dto.QualificationConclusion;
                }

                grade.StrategicTasks = strategicTasks;
                grade.StrategicTasksConclusion = dto.StrategicTasksConclusion;
                grade.Kpis = kpis;
                grade.KPIsConclusion = dto.KPIsConclusion;
                grade.Projects = projects;
                grade.Marks = marks;

                grade.IsProjectsFinalized = dto.IsProjectsFinalized;
                grade.IsStrategicTasksFinalized = dto.IsStrategicTasksFinalized;
                grade.IsKpisFinalized = dto.IsKpisFinalized;
                grade.IsMarksFinalized = dto.IsMarksFinalized;
                grade.IsQualificationFinalized = dto.IsQualificationFinalized;
                grade.IsValueJudgmentFinalized = dto.IsValueJudgmentFinalized;

                _unitOfWork.Grades.Update(grade);
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
                    Description = $"[GradeService.EditGrade] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        public async Task<IBaseResponse<object>> DeleteStrategicTask(int id)
        {
            try
            {
                var strategicTaskToDelete = await _unitOfWork.StrategicTasks.GetAsync(x => x.Id == id);

                if (strategicTaskToDelete is null)
                {
                    return new BaseResponse<object>()
                    {
                        StatusCode = StatusCodes.EntityNotFound,
                        Description = $"Стратегическая задача с id {id} не найдена."
                    };
                }

                _unitOfWork.StrategicTasks.Remove(strategicTaskToDelete);
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
                    Description = $"[GradeService.DeleteStrategicTask] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        public async Task<IBaseResponse<object>> DeleteProject(int id)
        {
            try
            {
                var projectToDelete = await _unitOfWork.Projects.GetAsync(x => x.Id == id);

                if (projectToDelete is null)
                {
                    return new BaseResponse<object>()
                    {
                        StatusCode = StatusCodes.EntityNotFound,
                        Description = $"Проект с id {id} не найден."
                    };
                }

                _unitOfWork.Projects.Remove(projectToDelete);
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
                    Description = $"[GradeService.DeleteProjectTask] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        public async Task<IBaseResponse<object>> DeleteKpi(int id)
        {
            try
            {
                var kpiToDelete = await _unitOfWork.Kpis.GetAsync(x => x.Id == id);

                if (kpiToDelete is null)
                {
                    return new BaseResponse<object>()
                    {
                        StatusCode = StatusCodes.EntityNotFound,
                        Description = $"КПЭ с id {id} не найден."
                    };
                }

                _unitOfWork.Kpis.Remove(kpiToDelete);
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
                    Description = $"[GradeService.DeleteKpi] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        public async Task<IBaseResponse<object>> DeleteMark(int id)
        {
            try
            {
                var markToDelete = await _unitOfWork.Marks.GetAsync(x => x.Id == id);

                if (markToDelete is null)
                {
                    return new BaseResponse<object>()
                    {
                        StatusCode = StatusCodes.EntityNotFound,
                        Description = $"Показатель с id {id} не найден."
                    };
                }

                _unitOfWork.Marks.Remove(markToDelete);
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
                    Description = $"[GradeService.DeleteMark] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        public async Task<IBaseResponse<object>> DeletePreviousJob(int id)
        {
            try
            {
                var previousJobToDelete = await _unitOfWork.PreviousJobs.GetAsync(x => x.Id == id);

                if (previousJobToDelete is null)
                {
                    return new BaseResponse<object>()
                    {
                        StatusCode = StatusCodes.EntityNotFound,
                        Description = $"Предыдущее место работы с id {id} не найдено."
                    };
                }

                _unitOfWork.PreviousJobs.Remove(previousJobToDelete);
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
                    Description = $"[GradeService.DeletePreviousJob] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }
    }
}