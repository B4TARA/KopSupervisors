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
        public DateOnly StartDate { get; set; } // дата начала качественной оценки
        public DateOnly? EndDate { get; set; } // дата завершения качественной оценки
        public DateOnly? NextAssessmentDate { get; set; } // дата начала следующей качественной оценки

        [Required]
        public SystemStatuses SystemStatus { get; set; } // системный статус качественной оценки


        public Employee Employee { get; set; } // пользователь, к которому относится данная качественная оценка
        public int EmployeeId { get; set; }



        public AssessmentType AssessmentType { get; set; } // Тип качественной оценки
        public int AssessmentTypeId { get; set; }



        public List<AssessmentResult> AssessmentResults { get; set; } = new(); // Результаты оценивания, относящиеся к данной качественной оценке



        public DateOnly DateOfCreation { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    }
}