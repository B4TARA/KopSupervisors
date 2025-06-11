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
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMappingService _mappingService;

        public GradeService(ApplicationDbContext context, IUnitOfWork unitOfWork, IMappingService mappingService)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _mappingService = mappingService;
        }

        public async Task<GradeExtendedDto> GetGradeDto(int gradeId, IEnumerable<GradeEntities> gradeEntities)
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

        public async Task EditGrade(GradeExtendedDto dto)
        {
            var grade = await _context.Grades.FirstOrDefaultAsync(x => x.Id == dto.Id);

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
                    SuccessRate = projectDto.SuccessRate ?? "",
                    AverageKpi = projectDto.AverageKpi ?? "",
                    SP = projectDto.SP ?? "",
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

            _context.Grades.Update(grade);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteStrategicTask(int id)
        {
            var strategicTaskToDelete = await _context.StrategicTasks.FirstOrDefaultAsync(x => x.Id == id);

            if (strategicTaskToDelete == null)
            {
                throw new Exception($"StrategicTask with ID {id} not found.");
            }

            _context.StrategicTasks.Remove(strategicTaskToDelete);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProject(int id)
        {
            var projectToDelete = await _context.Projects.FirstOrDefaultAsync(x => x.Id == id);

            if (projectToDelete == null)
            {
                throw new Exception($"Project with ID {id} not found.");
            }

            _context.Projects.Remove(projectToDelete);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteKpi(int id)
        {
            var kpiToDelete = await _context.Kpis.FirstOrDefaultAsync(x => x.Id == id);

            if (kpiToDelete == null)
            {
                throw new Exception($"KPI with ID {id} not found.");
            }

            _context.Kpis.Remove(kpiToDelete);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteMark(int id)
        {
            var markToDelete = await _context.Marks.FirstOrDefaultAsync(x => x.Id == id);

            if (markToDelete == null)
            {
                throw new Exception($"Mark with ID {id} not found.");
            }

            _context.Marks.Remove(markToDelete);
            await _context.SaveChangesAsync();
        }     

        public async Task<GradeDto?> GetLatestGradeForUser(int userId)
        {
            var latestGrade = await _context.Grades
                .AsNoTracking()
                .Where(g => g.UserId == userId)
                .OrderByDescending(g => g.Number)
                .Select(g => new GradeDto
                {
                    Id = g.Id,
                    Number = g.Number,
                    Period = $"{g.StartDate.ToString("dd.MM.yyyy")} - {g.EndDate.ToString("dd.MM.yyyy")}",
                    DateOfCreation = g.DateOfCreation.ToString("dd.MM.yyyy"),
                })
                .FirstOrDefaultAsync();

            return latestGrade;
        }

        public int CalculateCompletedCriteriaCount(Grade grade)
        {
            var count = 0;

            // Т.к. мероприятия не заполняются, а экспортируются из excel
            count++;

            if (grade.IsKpisFinalized) count++;
            if (grade.IsCorporateCompetenciesFinalized) count++;
            if (grade.IsManagmentCompetenciesFinalized) count++;
            if (grade.IsMarksFinalized) count++;
            if (grade.IsProjectsFinalized) count++;
            if (grade.IsQualificationFinalized) count++;
            if (grade.IsStrategicTasksFinalized) count++;
            if (grade.IsValueJudgmentFinalized) count++;

            return count; 
        }
    }
}