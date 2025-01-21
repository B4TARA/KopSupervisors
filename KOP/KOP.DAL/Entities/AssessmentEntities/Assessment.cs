using KOP.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace KOP.DAL.Entities.AssessmentEntities
{
    public class Assessment
    {
        [Key]
        public int Id { get; set; } // id качественной оценки

        [Required]
        public int Number { get; set; } // номер качественной оценки (очередность)

        [Required]
        public SystemStatuses SystemStatus { get; set; } // системный статус качественной оценки

        public User User { get; set; } // пользователь, к которому относится данная качественная оценка
        public int UserId { get; set; }

        public AssessmentType AssessmentType { get; set; } // Тип качественной оценки
        public int AssessmentTypeId { get; set; }

        public List<AssessmentResult> AssessmentResults { get; set; } = new(); // Результаты оценивания, относящиеся к данной качественной оценке

        public DateOnly DateOfCreation { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    }
}