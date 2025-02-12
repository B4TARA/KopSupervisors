using KOP.Common.Enums;

namespace KOP.DAL.Entities.GradeEntities
{
    public class Grade
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public SystemStatuses SystemStatus { get; set; }

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

        public User User { get; set; }
        public int UserId { get; set; }

        public Qualification Qualification { get; set; }
        public int QualificationId { get; set; }

        public ValueJudgment ValueJudgment { get; set; }
        public int ValueJudgmentId { get; set; }

        public List<Mark> Marks { get; set; } = new();
        public List<Kpi> Kpis { get; set; } = new();
        public List<Project> Projects { get; set; } = new();
        public List<StrategicTask> StrategicTasks { get; set; } = new();
        public List<TrainingEvent> TrainingEvents { get; set; } = new();

        public DateOnly DateOfCreation { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    }
}