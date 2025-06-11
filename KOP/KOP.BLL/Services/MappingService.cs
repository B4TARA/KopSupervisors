using KOP.BLL.Interfaces;
using KOP.Common.Dtos.AssessmentDtos;
using KOP.Common.Dtos.AssessmentResultDtos;
using KOP.Common.Dtos.GradeDtos;
using KOP.Common.Dtos.UserDtos;
using KOP.Common.Enums;
using KOP.DAL.Entities;

namespace KOP.BLL.Services
{
    public class MappingService : IMappingService
    {
        public GradeExtendedDto CreateGradeDto(Grade grade, IEnumerable<MarkType> allMarkTypes)
        {
            var dto = new GradeExtendedDto()
            {
                Id = grade.Id,
                Number = grade.Number,
                StartDate = grade.StartDate,
                EndDate = grade.EndDate,
                SystemStatus = grade.SystemStatus,
                GradeStatus = grade.GradeStatus,
                IsProjectsFinalized = grade.IsProjectsFinalized,
                IsStrategicTasksFinalized = grade.IsStrategicTasksFinalized,
                IsKpisFinalized = grade.IsKpisFinalized,
                IsMarksFinalized = grade.IsMarksFinalized,
                IsQualificationFinalized = grade.IsQualificationFinalized,
                IsValueJudgmentFinalized = grade.IsValueJudgmentFinalized,
                StrategicTasksConclusion = grade.StrategicTasksConclusion,
                KPIsConclusion = grade.KPIsConclusion,
                QualificationConclusion = grade.QualificationConclusion,
                IsCorporateCompetenciesFinalized = grade.IsCorporateCompetenciesFinalized,
                IsManagmentCompetenciesFinalized = grade.IsManagmentCompetenciesFinalized,
                ManagmentCompetenciesConclusion = grade.ManagmentCompetenciesConclusion,
                CorporateCompetenciesConclusion = grade.CorporateCompetenciesConclusion,
                Qn2 = grade.Qn2,
                UserId = grade.UserId,
            };

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

                    markTypeDto.Marks.Add(markDto);
                }

                dto.MarkTypeDtoList.Add(markTypeDto);
            }

            foreach (var kpi in grade.Kpis)
            {
                var kpiDto = CreateKpiDto(kpi);

                dto.KpiDtoList.Add(kpiDto);
            }

            foreach (var project in grade.Projects)
            {
                var projectDto = CreateProjectDto(project);

                dto.ProjectDtoList.Add(projectDto);
            }

            foreach (var strategicTask in grade.StrategicTasks)
            {
                var strategicTaskDto = CreateStrategicTaskDto(strategicTask);

                dto.StrategicTaskDtoList.Add(strategicTaskDto);
            }

            foreach (var trainingEvent in grade.TrainingEvents)
            {
                var trainingEventDto = CreateTrainingEventDto(trainingEvent);

                dto.TrainingEventDtoList.Add(trainingEventDto);
            }

            foreach (var assessment in grade.Assessments)
            {
                var assessmentDto = CreateAssessmentDto(assessment);

                dto.AssessmentDtoList.Add(assessmentDto);
            }

            return dto;
        }

        public MarkDto CreateMarkDto(Mark mark)
        {
            var dto = new MarkDto()
            {
                Id = mark.Id,
                PercentageValue = mark.PercentageValue,
                Period = mark.Period,
            };

            return dto;
        }

        public KpiDto CreateKpiDto(Kpi kpi)
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

            return dto;
        }

        public ProjectDto CreateProjectDto(Project project)
        {
            var dto = new ProjectDto()
            {
                Id = project.Id,
                Name = project.Name,
                UserRole = project.UserRole,
                Stage = project.Stage,
                StartDateTime = project.StartDate.ToDateTime(TimeOnly.MinValue),
                EndDateTime = project.EndDate.ToDateTime(TimeOnly.MinValue),
                SuccessRate = project.SuccessRate,
                AverageKpi = project.AverageKpi,
                SP = project.SP,
            };

            return dto;
        }

        public StrategicTaskDto CreateStrategicTaskDto(StrategicTask strategicTask)
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

            return dto;
        }

        public TrainingEventDto CreateTrainingEventDto(TrainingEvent trainingEvent)
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

            return dto;
        }

        public AssessmentDto CreateAssessmentDto(Assessment assessment)
        {

            var dto = new AssessmentDto()
            {
                Id = assessment.Id,
                UserId = assessment.UserId,
                SystemStatus = assessment.SystemStatus,
                SystemAssessmentType = assessment.AssessmentType.SystemAssessmentType,
            };

            var allAssessmentResults = assessment.AssessmentResults;

            foreach (var result in allAssessmentResults)
            {
                var resultDto = CreateAssessmentResultDto(result, assessment.AssessmentType);

                dto.AllAssessmentResults.Add(resultDto);
            }

            var completedAssessmentResults = assessment.AssessmentResults.Where(x => x.SystemStatus == SystemStatuses.COMPLETED);

            foreach (var result in completedAssessmentResults)
            {
                var resultDto = CreateAssessmentResultDto(result, assessment.AssessmentType);

                dto.CompletedAssessmentResults.Add(resultDto);
                dto.SumValue += resultDto.Sum;
            }

            if (dto.CompletedAssessmentResults.Any())
            {
                dto.AverageValue = dto.SumValue / dto.CompletedAssessmentResults.Count();
            }

            foreach (var interpretation in assessment.AssessmentType.AssessmentInterpretations)
            {
                var interpretationDto = CreateAssessmentInterpretationDto(interpretation);

                if (dto.AverageValue >= interpretationDto.MinValue && dto.AverageValue <= interpretationDto.MaxValue)
                {
                    dto.AverageAssessmentInterpretation = interpretationDto;
                }
            }

            return dto;
        }

        public AssessmentResultDto CreateAssessmentResultDto(AssessmentResult result, AssessmentType type)
        {
            var dto = new AssessmentResultDto()
            {
                Id = result.Id,
                SystemStatus = result.SystemStatus,
                Type = result.Type,
                Sum = result.AssessmentResultValues.Sum(x => x.Value),
                AssignedBy = result.AssignedBy
            };

            var assessmentInterpretation = type.AssessmentInterpretations.FirstOrDefault(x => x.MinValue <= dto.Sum && x.MaxValue >= dto.Sum);
            if (assessmentInterpretation != null)
            {
                dto.HtmlClassName = assessmentInterpretation.HtmlClassName;
            }

            dto.Judge = new UserExtendedDto
            {
                Id = result.Judge.Id,
                FullName = result.Judge.FullName,
                SystemRoles = result.Judge.SystemRoles,
                ImagePath = result.Judge.ImagePath,
            };

            dto.Judged = new UserExtendedDto
            {
                Id = result.Assessment.User.Id,
                FullName = result.Assessment.User.FullName,
            };

            foreach (var value in result.AssessmentResultValues)
            {
                var valueDto = CreateAssessmentResultValueDto(value);

                dto.Values.Add(valueDto);
            }

            foreach (var element in type.AssessmentMatrix.Elements)
            {
                var elementDto = CreateAssessmentMatrixElementDto(element);

                dto.Elements.Add(elementDto);
            }

            dto.ElementsByRow = dto.Elements.OrderBy(x => x.Column).GroupBy(x => x.Row).OrderBy(x => x.Key).ToList();
            dto.MaxValue = type.AssessmentMatrix.MaxAssessmentMatrixResultValue;
            dto.MinValue = type.AssessmentMatrix.MinAssessmentMatrixResultValue;

            return dto;
        }

        public AssessmentInterpretationDto CreateAssessmentInterpretationDto(AssessmentInterpretation interpretation)
        {
            var dto = new AssessmentInterpretationDto()
            {
                MinValue = interpretation.MinValue,
                MaxValue = interpretation.MaxValue,
                Competence = interpretation.Competence,
                Level = interpretation.Level,
                HtmlClassName = interpretation.HtmlClassName,
            };

            return dto;
        }

        public AssessmentResultValueDto CreateAssessmentResultValueDto(AssessmentResultValue value)
        {
            var dto = new AssessmentResultValueDto()
            {
                Value = value.Value,
                AssessmentMatrixRow = value.AssessmentMatrixRow,
            };

            return dto;
        }

        public AssessmentMatrixElementDto CreateAssessmentMatrixElementDto(AssessmentMatrixElement element)
        {
            var dto = new AssessmentMatrixElementDto()
            {
                Column = element.Column,
                Row = element.Row,
                Value = element.Value,
                HtmlClassName = element.HtmlClassName,
            };

            return dto;
        }
    }
}