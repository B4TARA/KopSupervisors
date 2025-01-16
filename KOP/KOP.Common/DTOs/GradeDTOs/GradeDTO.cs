using KOP.Common.Enums;

namespace KOP.Common.DTOs.GradeDTOs
{
    public class GradeDTO
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }

        public string? StrategicTasksConclusion { get; set; }
        public string? KPIsConclusion { get; set; }
        public string? QualificationConclusion { get; set; }
        public string? ManagmentCompetenciesConclusion { get; set; }
        public string? CorporateCompetenciesConclusion { get; set; }

        public QualificationDTO? Qualification { get; set; }
        public ValueJudgmentDTO? ValueJudgment { get; set; }
        public List<MarkTypeDTO> MarkTypes { get; set; } = new();
        public List<KpiDTO> Kpis { get; set; } = new();
        public List<ProjectDTO> Projects { get; set; } = new();
        public List<StrategicTaskDTO> StrategicTasks { get; set; } = new();
        public List<TrainingEventDTO> TrainingEvents { get; set; } = new();

        public SystemStatuses SystemStatus { get; set; }
    }
}