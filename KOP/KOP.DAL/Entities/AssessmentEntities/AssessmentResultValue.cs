using System.ComponentModel.DataAnnotations;

namespace KOP.DAL.Entities.AssessmentEntities
{
    public class AssessmentResultValue
    {
        [Key]
        public int Id { get; set; } // id значения результата качественной оценки

        [Required]
        public int Value { get; set; } // значение результата качественной оценки

        [Required]
        public int AssessmentMatrixRow { get; set; } // строка матрицы, к которой относится данное значение



        public AssessmentResult AssessmentResult { get; set; } // Результат качественной оценки, к которому относится данный результат
        public int AssessmentResultId { get; set; }



        public DateOnly DateOfCreation { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    }
}