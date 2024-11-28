using KOP.BLL.Interfaces;
using KOP.Common.DTOs;
using KOP.Common.DTOs.GradeDTOs;
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

        // Получить количественную оценку по id вместе с нужными данными
        public async Task<IBaseResponse<GradeDTO>> GetGrade(int gradeId, List<GradeEntities> gradeEntitiesList)
        {
            try
            {
                var includeProperties = new List<string>();

                foreach (var gradeEntity in gradeEntitiesList)
                {
                    var strGradeEntity = Enum.GetName(typeof(GradeEntities), gradeEntity);

                    if (strGradeEntity is null)
                    {
                        continue;
                    }

                    includeProperties.Add(strGradeEntity);
                }

                var grade = await _unitOfWork.Grades.GetAsync(x => x.Id == gradeId, includeProperties: includeProperties.ToArray());

                if (grade == null)
                {
                    return new BaseResponse<GradeDTO>()
                    {
                        Description = $"[GradeService.GetStrategicTasks] : Оценка с id = {gradeId} не найдена",
                        StatusCode = StatusCodes.EntityNotFound,
                    };
                }

                var dto = _mappingService.CreateGradeDTO(grade);

                if (dto.StatusCode != StatusCodes.OK || dto.Data == null)
                {
                    return new BaseResponse<GradeDTO>()
                    {
                        StatusCode = dto.StatusCode,
                        Description = dto.Description
                    };
                }

                return new BaseResponse<GradeDTO>()
                {
                    StatusCode = StatusCodes.OK,
                    Data = dto.Data
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<GradeDTO>()
                {
                    Description = $"[GradeService.GetStrategicTasks] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        // Создать стратегическую задачу
        public async Task<IBaseResponse<object>> CreateStrategicTask(StrategicTaskDTO dto)
        {
            try
            {
                var strategicTask = new StrategicTask
                {
                    Name = dto.Name,
                    Purpose = dto.Purpose,
                    PlanDate = dto.PlanDate,
                    FactDate = dto.FactDate,
                    PlanResult = dto.PlanResult,
                    FactResult = dto.FactResult,
                    Remark = dto.Remark,
                    GradeId = dto.GradeId,
                };

                await _unitOfWork.StrategicTasks.AddAsync(strategicTask);
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
                    Description = $"[GradeService.CreateStrategicTask] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        // Создать обучающее мероприятие
        public async Task<IBaseResponse<object>> CreateTrainingEvent(TrainingEventDTO dto)
        {
            try
            {
                var trainingEvent = new TrainingEvent
                {
                    Name = dto.Name,
                    Status = dto.Status,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    Competence = dto.Competence,
                    EmployeeId = dto.EmployeeId,
                    GradeId = dto.GradeId,
                };

                await _unitOfWork.TrainingEvents.AddAsync(trainingEvent);
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
                    Description = $"[GradeService.CreateTrainingEvent] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        // Создать проект
        public async Task<IBaseResponse<object>> CreateProject(ProjectDTO dto)
        {
            try
            {
                var project = new Project
                {
                    Name = dto.Name,
                    SupervisorSspName = dto.SupervisorSspName,
                    Stage = dto.Stage,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    CurrentDate = dto.CurrentDate,
                    CurrentMonth = dto.CurrentMonth,
                    CurrentYear = dto.CurrentYear,
                    PlanStages = dto.PlanStages,
                    FactStages = dto.FactStages,
                    SPn = dto.SPn,
                    GradeId = dto.GradeId,
                };

                await _unitOfWork.Projects.AddAsync(project);
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
                    Description = $"[GradeService.CreateProject] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        // Создать КПЭ
        public async Task<IBaseResponse<object>> CreateKpi(KpiDTO dto)
        {
            try
            {
                var kpi = new Kpi
                {
                    Name = dto.Name,
                    PeriodStartDate = dto.PeriodStartDate,
                    PeriodEndDate = dto.PeriodEndDate,
                    CompletionPercentage = dto.CompletionPercentage,
                    CalculationMethod = dto.CalculationMethod,
                    EmployeeId = dto.EmployeeId,
                    GradeId = dto.GradeId,
                };

                await _unitOfWork.Kpis.AddAsync(kpi);
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
                    Description = $"[GradeService.CreateKpi] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        // Создать показатель
        public async Task<IBaseResponse<object>> CreateMark(MarkDTO dto)
        {
            try
            {
                var mark = new Mark
                {
                    PercentageValue = dto.PercentageValue,
                    Period = dto.Period,
                    MarkTypeId = dto.MarkTypeDTO.Id,
                    EmployeeId = dto.EmployeeId,
                    GradeId = dto.GradeId,
                };

                await _unitOfWork.Marks.AddAsync(mark);
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
                    Description = $"[GradeService.CreateMark] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }




        // Изменить стратегическую задачу
        public async Task<IBaseResponse<object>> EditStrategicTask(StrategicTaskDTO dto)
        {
            try
            {
                var strategicTask = await _unitOfWork.StrategicTasks.GetAsync(x => x.Id == dto.Id);

                if (strategicTask is null)
                {
                    return new BaseResponse<object>()
                    {
                        Description = $"[GradeService.EditStrategicTask] : Стратегическая задача с id = {dto.Id} не найдена",
                        StatusCode = StatusCodes.EntityNotFound
                    };
                }

                strategicTask.Name = dto.Name;
                strategicTask.Purpose = dto.Purpose;
                strategicTask.PlanDate = dto.PlanDate;
                strategicTask.FactDate = dto.FactDate;
                strategicTask.PlanResult = dto.PlanResult;
                strategicTask.FactResult = dto.FactResult;
                strategicTask.Remark = dto.Remark;

                _unitOfWork.StrategicTasks.Update(strategicTask);
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
                    Description = $"[GradeService.EditStrategicTask] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        // Изменить обучающее мероприятие
        public async Task<IBaseResponse<object>> EditTrainingEvent(TrainingEventDTO dto)
        {
            try
            {
                var trainingEvent = await _unitOfWork.TrainingEvents.GetAsync(x => x.Id == dto.Id);

                if (trainingEvent is null)
                {
                    return new BaseResponse<object>()
                    {
                        Description = $"[GradeService.EditTrainingEvent] : Обучающее мероприятие с id = {dto.Id} не найдено",
                        StatusCode = StatusCodes.EntityNotFound
                    };
                }

                trainingEvent.Name = dto.Name;
                trainingEvent.Status = dto.Status;
                trainingEvent.StartDate = dto.StartDate;
                trainingEvent.EndDate = dto.EndDate;
                trainingEvent.Competence = dto.Competence;

                _unitOfWork.TrainingEvents.Update(trainingEvent);
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
                    Description = $"[GradeService.EditTrainingEvent] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        // Изменить квалификацию
        public async Task<IBaseResponse<object>> EditQualification(QualificationDTO dto)
        {
            try
            {
                var qualification = await _unitOfWork.Qualifications.GetAsync(x => x.Id == dto.Id);

                if (qualification is null)
                {
                    return new BaseResponse<object>()
                    {
                        Description = $"[GradeService.EditQualification] : Квалификация с id = {dto.Id} не найдена",
                        StatusCode = StatusCodes.EntityNotFound
                    };
                }

                qualification.SupervisorSspName = dto.SupervisorSspName;
                qualification.Link = dto.Link;
                qualification.HigherEducation = dto.HigherEducation;
                qualification.Speciality = dto.Speciality;
                qualification.QualificationResult = dto.QualificationResult;
                qualification.StartDate = dto.StartDate;
                qualification.EndDate = dto.EndDate;
                qualification.AdditionalEducation = dto.AdditionalEducation;
                qualification.CurrentDate = dto.CurrentDate;
                qualification.ExperienceMonths = dto.ExperienceMonths;
                qualification.ExperienceYears = dto.ExperienceYears;
                qualification.PreviousPosition1 = dto.PreviousPosition1;
                qualification.PreviousPosition2 = dto.PreviousPosition2;
                qualification.CurrentPosition = dto.CurrentPosition;
                qualification.EmploymentContarctTerminations = dto.EmploymentContarctTerminations;

                _unitOfWork.Qualifications.Update(qualification);
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
                    Description = $"[GradeService.EditQualification] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        // Изменить оценочное суждение
        public async Task<IBaseResponse<object>> EditValueJudgment(ValueJudgmentDTO dto)
        {
            try
            {
                var valueJudgment = await _unitOfWork.ValueJudgments.GetAsync(x => x.Id == dto.Id);

                if (valueJudgment is null)
                {
                    return new BaseResponse<object>()
                    {
                        Description = $"[GradeService.EditValueJudgment] : Оценочное суждение с id = {dto.Id} не найдено",
                        StatusCode = StatusCodes.EntityNotFound
                    };
                }

                valueJudgment.Strengths = dto.Strengths;
                valueJudgment.BehaviorToCorrect = dto.BehaviorToCorrect;
                valueJudgment.RecommendationsForDevelopment = dto.RecommendationsForDevelopment;

                _unitOfWork.ValueJudgments.Update(valueJudgment);
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
                    Description = $"[GradeService.EditValueJudgment] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }
    }
}