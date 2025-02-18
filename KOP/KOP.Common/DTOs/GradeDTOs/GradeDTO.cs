using KOP.Common.Dtos.AssessmentDtos;
using KOP.Common.Enums;

namespace KOP.Common.Dtos.GradeDtos
{
    public class GradeDto
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }

        public bool IsProjectsFinalized { get; set; }
        public bool IsStrategicTasksFinalized { get; set; }
        public bool IsKpisFinalized { get; set; }
        public bool IsMarksFinalized { get; set; }
        public bool IsQualificationFinalized { get; set; }
        public bool IsValueJudgmentFinalized { get; set; }

        public string? StrategicTasksConclusion { get; set; }
        public string? KPIsConclusion { get; set; }
        public string? QualificationConclusion { get; set; }
        public string? ManagmentCompetenciesConclusion { get; set; }
        public string? CorporateCompetenciesConclusion { get; set; }

        public QualificationDto? Qualification { get; set; }
        public ValueJudgmentDto? ValueJudgment { get; set; }
        public List<MarkTypeDto> MarkTypes { get; set; } = new();
        public List<KpiDto> Kpis { get; set; } = new();
        public List<ProjectDto> Projects { get; set; } = new();
        public List<StrategicTaskDto> StrategicTasks { get; set; } = new();
        public List<TrainingEventDto> TrainingEvents { get; set; } = new();
        public List<AssessmentDto> AssessmentDtos { get; set; } = new();

        public SystemStatuses SystemStatus { get; set; }
        public int UserId { get; set; }
    }
}