using KOP.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace KOP.DAL.Entities.AssessmentEntities
{
    public class AssessmentResult
    {
        [Key]
        public int Id { get; set; } // id результата качественной оценки оценщиком

        public DateOnly? ResultDate { get; set; } // дата качественной оценки оценщиком (может быть null - оценка не завершена)

        [Required]
        public SystemStatuses SystemStatus { get; set; } // Текущий статус качественной оценки



        public Assessment Assessment { get; set; } // оценка, к которой относится данный результат оценки 
        public int AssessmentId { get; set; }



        public User Judge { get; set; } // оценщик
        public int JudgeId { get; set; }



        public User Judged { get; set; } // оцениваемый
        public int JudgedId { get; set; }



        public List<AssessmentResultValue> AssessmentResultValues { get; set; } = new(); // Значения, относящиеся к этому результату



        public DateOnly DateOfCreation { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    }
}