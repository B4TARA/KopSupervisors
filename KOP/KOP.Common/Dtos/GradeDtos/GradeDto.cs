using KOP.Common.Enums;

namespace KOP.Common.Dtos.GradeDtos
{
    public class GradeDto
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public string UserFullName { get; set; }
        public bool IsPending { get; set; }
        public string Period { get; set; }  
        public string DateOfCreation { get; set; }

        public bool IsProjectsFinalized { get; set; }
        public bool IsStrategicTasksFinalized { get; set; }
        public bool IsKpisFinalized { get; set; }
        public bool IsMarksFinalized { get; set; }
        public bool IsQualificationFinalized { get; set; }
        public bool IsValueJudgmentFinalized { get; set; }
        public bool IsCorporateCompetenciesFinalized { get; set; }
        public bool IsManagmentCompetenciesFinalized { get; set; }

        public int CompletedСriteriaCount { get; set; }
        public GradeStatuses GradeStatus { get; set; }
    }
}