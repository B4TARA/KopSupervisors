using System.ComponentModel.DataAnnotations;

namespace KOP.DAL.Entities.GradeEntities
{
    public class TrainingEvent
    {
        [Key]
        public int Id { get; set; } // Id обучающего мероприятия

        [Required]
        public string Name { get; set; } // Наименование мероприятия

        [Required]
        public string Status { get; set; } // Статус мероприятия

        [Required]
        public DateOnly StartDate { get; set; } // Дата начала мероприятия

        [Required]
        public DateOnly EndDate { get; set; } // Дата завершения мероприятия

        [Required]
        public string Competence { get; set; } // Компетенция



        public Grade Grade { get; set; } // Оценка, к которой относится данное мероприятие
        public int GradeId { get; set; }



        public DateOnly DateOfCreation { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    }
}