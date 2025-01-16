using System.ComponentModel.DataAnnotations;

namespace KOP.DAL.Entities.GradeEntities
{
    public class PreviousJob
    {
        [Key]
        public int Id { get; set; } // Id предыдущего места работы

        [Required]
        public DateOnly StartDate { get; set; } // Период работы "с"

        [Required]
        public DateOnly EndDate { get; set; } // Период работы "по"

        [Required]
        public string OrganizationName { get; set; } // Наименование организации

        [Required]
        public string PositionName { get; set; } // Наименование должности



        public Qualification Qualification { get; set; } // Квалификация, к которой относится данная предыдущая работа
        public int QualificationId { get; set; }




        public DateOnly DateOfCreation { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    }
}