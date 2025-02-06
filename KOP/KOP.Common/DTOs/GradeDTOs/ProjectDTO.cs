using System.ComponentModel.DataAnnotations;

namespace KOP.Common.Dtos.GradeDtos
{
    public class ProjectDto
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string SupervisorSspName { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string Stage { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public DateTime StartDateTime { get; set; }
        public DateOnly StartDate => DateOnly.FromDateTime(StartDateTime);

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public DateTime EndDateTime { get; set; }
        public DateOnly EndDate => DateOnly.FromDateTime(EndDateTime);

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public DateTime CurrentStatusDateTime { get; set; }
        public DateOnly CurrentStatusDate => DateOnly.FromDateTime(CurrentStatusDateTime);

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public int PlanStages { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public int FactStages { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public int SPn { get; set; }
    }
}