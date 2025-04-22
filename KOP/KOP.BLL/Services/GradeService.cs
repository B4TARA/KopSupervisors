using KOP.BLL.Interfaces;
using KOP.Common.Dtos.GradeDtos;
using KOP.Common.Enums;
using KOP.DAL;
using KOP.DAL.Entities;
using KOP.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KOP.BLL.Services
{
    public class GradeService : IGradeService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMappingService _mappingService;

        public GradeService(ApplicationDbContext dbContext, IUnitOfWork unitOfWork, IMappingService mappingService)
        {
            _dbContext = dbContext;
            _unitOfWork = unitOfWork;
            _mappingService = mappingService;
        }

        public async Task<GradeDto> GetGradeDto(int gradeId, IEnumerable<GradeEntities> gradeEntities)
        {
            var includeProperties = new List<string>();
            var allMarkTypes = new List<MarkType>();

            foreach (var gradeEntity in gradeEntities)
            {
                var strGradeEntity = Enum.GetName(typeof(GradeEntities), gradeEntity);

                if (strGradeEntity == null)
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
                    // КОСТЫЛЬ
                    includeProperties.Add(strGradeEntity + ".HigherEducations");
                    strGradeEntity += ".PreviousJobs";
                }
                // УБРАТЬ ЭТОТ КОСТЫЛЬ !!!
                else if (gradeEntity == GradeEntities.Assessments)
                {
                    strGradeEntity += ".AssessmentType.AssessmentMatrix";
                }

                includeProperties.Add(strGradeEntity);
            }

            var grade = await _unitOfWork.Grades.GetAsync(x => x.Id == gradeId, includeProperties: includeProperties.ToArray());
            if (grade == null)
            {
                throw new Exception($"Grade with ID {gradeId} not found.");
            }

            var gradeDto = _mappingService.CreateGradeDto(grade, allMarkTypes);

            return gradeDto;
        }
  
        public async Task EditGrade(GradeDto dto)
        {
            var grade = await _dbContext.Grades.FirstOrDefaultAsync(x => x.Id == dto.Id);

            if (grade == null)
            {
                throw new Exception($"Grade with ID {dto.Id} not found.");
            }

            var strategicTasks = new List<StrategicTask>();
            var kpis = new List<Kpi>();
            var projects = new List<Project>();
            var marks = new List<Mark>();

            foreach (var strategicTaskDto in dto.StrategicTaskDtoList)
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

            foreach (var kpiDto in dto.KpiDtoList)
            {
                var kpi = new Kpi
                {
                    Name = kpiDto.Name ?? "КПЭ",
                    PeriodStartDate = kpiDto.PeriodStartDate,
                    PeriodEndDate = kpiDto.PeriodEndDate,
                    CompletionPercentage = kpiDto.CompletionPercentage ?? "% выполнения",
                    CalculationMethod = kpiDto.CalculationMethod ?? "Методика расчета",
                };

                kpis.Add(kpi);
            }

            foreach (var projectDto in dto.ProjectDtoList)
            {
                var project = new Project
                {
                    UserRole = projectDto.UserRole ?? "руководителем/заказчиком/со-заказчиком",
                    Name = projectDto.Name ?? "Наименование проекта",
                    Stage = projectDto.Stage ?? "в реализации/завершен",
                    StartDate = projectDto.StartDate,
                    EndDate = projectDto.EndDate,
                    SuccessRate = projectDto.SuccessRate,
                    AverageKpi = projectDto.AverageKpi,
                    SP = projectDto.SP,
                };

                projects.Add(project);
            }

            foreach (var markTypeDto in dto.MarkTypeDtoList)
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

            if (dto.ValueJudgmentDto != null)
            {
                grade.ValueJudgment.Strengths = dto.ValueJudgmentDto.Strengths ?? "";
                grade.ValueJudgment.BehaviorToCorrect = dto.ValueJudgmentDto.BehaviorToCorrect ?? "";
                grade.ValueJudgment.RecommendationsForDevelopment = dto.ValueJudgmentDto.RecommendationsForDevelopment ?? "";
            }

            if (dto.QualificationDto != null)
            {
                grade.Qualification.CurrentStatusDate = dto.QualificationDto.CurrentStatusDate;
                grade.Qualification.CurrentExperienceYears = dto.QualificationDto.CurrentExperienceYears;
                grade.Qualification.CurrentExperienceMonths = dto.QualificationDto.CurrentExperienceMonths;
                grade.Qualification.CurrentJobStartDate = dto.QualificationDto.CurrentJobStartDate;
                grade.Qualification.CurrentJobPositionName = dto.QualificationDto.CurrentJobPositionName ?? "";
                grade.Qualification.EmploymentContarctTerminations = dto.QualificationDto.EmploymentContarctTerminations ?? "";
                grade.Qualification.QualificationResult = dto.QualificationDto.QualificationResult ?? "";

                grade.Qualification.PreviousJobs.Clear();
                foreach (var previousJob in dto.QualificationDto.PreviousJobs)
                {
                    grade.Qualification.PreviousJobs.Add(new PreviousJob
                    {
                        StartDate = previousJob.StartDate,
                        EndDate = previousJob.EndDate,
                        OrganizationName = previousJob.OrganizationName ?? "",
                        PositionName = previousJob.PositionName ?? "",
                    });
                }

                grade.Qualification.HigherEducations.Clear();
                foreach (var higherEducation in dto.QualificationDto.HigherEducations)
                {
                    grade.Qualification.HigherEducations.Add(new HigherEducation
                    {
                        Education = higherEducation.Education ?? "",
                        Speciality = higherEducation.Speciality ?? "",
                        QualificationName = higherEducation.QualificationName ?? "",
                        StartDate = higherEducation.StartDate,
                        EndDate = higherEducation.EndDate,
                    });
                }

                grade.QualificationConclusion = dto.QualificationConclusion;
            }

            grade.StrategicTasks = strategicTasks;
            grade.StrategicTasksConclusion = dto.StrategicTasksConclusion;
            grade.Kpis = kpis;
            grade.KPIsConclusion = dto.KPIsConclusion;
            grade.Projects = projects;
            grade.Qn2 = dto.Qn2;
            grade.Marks = marks;

            grade.IsProjectsFinalized = dto.IsProjectsFinalized;
            grade.IsStrategicTasksFinalized = dto.IsStrategicTasksFinalized;
            grade.IsKpisFinalized = dto.IsKpisFinalized;
            grade.IsMarksFinalized = dto.IsMarksFinalized;
            grade.IsQualificationFinalized = dto.IsQualificationFinalized;

            _dbContext.Grades.Update(grade);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteStrategicTask(int id)
        {
            var strategicTaskToDelete = await _dbContext.StrategicTasks.FirstOrDefaultAsync(x => x.Id == id);

            if (strategicTaskToDelete == null)
            {
                throw new Exception($"StrategicTask with ID {id} not found.");
            }

            _dbContext.StrategicTasks.Remove(strategicTaskToDelete);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteProject(int id)
        {
            var projectToDelete = await _dbContext.Projects.FirstOrDefaultAsync(x => x.Id == id);

            if (projectToDelete == null)
            {
                throw new Exception($"Project with ID {id} not found.");
            }

            _dbContext.Projects.Remove(projectToDelete);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteKpi(int id)
        {
            var kpiToDelete = await _dbContext.Kpis.FirstOrDefaultAsync(x => x.Id == id);

            if (kpiToDelete == null)
            {
                throw new Exception($"KPI with ID {id} not found.");
            }

            _dbContext.Kpis.Remove(kpiToDelete);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteMark(int id)
        {
            var markToDelete = await _dbContext.Marks.FirstOrDefaultAsync(x => x.Id == id);

            if (markToDelete == null)
            {
                throw new Exception($"Mark with ID {id} not found.");
            }

            _dbContext.Marks.Remove(markToDelete);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeletePreviousJob(int id)
        {
            var previousJobToDelete = await _dbContext.PreviousJobs.FirstOrDefaultAsync(x => x.Id == id);

            if (previousJobToDelete == null)
            {
                throw new Exception($"PreviousJob with ID {id} not found.");
            }

            _dbContext.PreviousJobs.Remove(previousJobToDelete);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteHigherEducation(int id)
        {
            var higherEducationToDelete = await _dbContext.HigherEducations.FirstOrDefaultAsync(x => x.Id == id);

            if (higherEducationToDelete == null)
            {
                throw new Exception($"HigherEducation with ID {id} not found.");
            }

            _dbContext.HigherEducations.Remove(higherEducationToDelete);
            await _dbContext.SaveChangesAsync();
        }
    }
}