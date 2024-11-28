using System.ComponentModel.DataAnnotations;

namespace KOP.Common.DTOs.GradeDTOs
{
    public class ProjectDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SupervisorSspName { get; set; }
        public string Stage { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int CurrentDate { get; set; }
        public int CurrentMonth { get; set; }
        public int CurrentYear { get; set; }
        public int PlanStages { get; set; }
        public int FactStages { get; set; }
        public int SPn { get; set; }
        public int GradeId { get; set; }
    }
}