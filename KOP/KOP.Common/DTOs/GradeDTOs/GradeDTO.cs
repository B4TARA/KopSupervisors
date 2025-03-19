using KOP.Common.Dtos.AssessmentDtos;
using KOP.Common.Enums;

namespace KOP.Common.Dtos.GradeDtos
{
    public class GradeDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int Number { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }

        public bool IsProjectsFinalized { get; set; }
        public bool IsStrategicTasksFinalized { get; set; }
        public bool IsKpisFinalized { get; set; }
        public bool IsMarksFinalized { get; set; }
        public bool IsQualificationFinalized { get; set; }
        public bool IsValueJudgmentFinalized { get; set; }
        public bool IsCorporateCompetenciesFinalized { get; set; }
        public bool IsManagmentCompetenciesFinalized { get; set; }

        public string? StrategicTasksConclusion { get; set; }
        public string? KPIsConclusion { get; set; }
        public string? QualificationConclusion { get; set; }
        public string? ManagmentCompetenciesConclusion { get; set; }
        public string? CorporateCompetenciesConclusion { get; set; }

        public double Qn2 { get; set; }

        public QualificationDto? QualificationDto { get; set; }
        public ValueJudgmentDto? ValueJudgmentDto { get; set; }
        public List<MarkTypeDto> MarkTypeDtoList { get; set; } = new();
        public List<KpiDto> KpiDtoList { get; set; } = new();
        public List<ProjectDto> ProjectDtoList { get; set; } = new();
        public List<StrategicTaskDto> StrategicTaskDtoList { get; set; } = new();
        public List<TrainingEventDto> TrainingEventDtoList { get; set; } = new();
        public List<AssessmentDto> AssessmentDtoList { get; set; } = new();

        public SystemStatuses SystemStatus { get; set; }
        public GradeStatuses GradeStatus { get; set; }
    }
}