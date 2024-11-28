using System.ComponentModel.DataAnnotations;

namespace KOP.DAL.Entities.AssessmentEntities
{
    public class AssessmentMatrix
    {
        [Key]
        public int Id { get; set; } // id матрицы качественной оценки

        [Required]
        public int MinAssessmentMatrixResultValue { get; set; } // Минимальное значение, которое может выставить оценщник

        [Required]
        public int MaxAssessmentMatrixResultValue { get; set; } // Максимальное значение, которое может выставить оценщник


        public List<AssessmentType> AssessmentTypes { get; set; } = new(); // Типы оценок, к которым относится данная матрица
        public List<AssessmentMatrixElement> Elements { get; set; } = new(); // Элементы матрицы качественной оценки



        public DateOnly DateOfCreation { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    }
}
