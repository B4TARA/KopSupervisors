using System.ComponentModel.DataAnnotations;

namespace KOP.DAL.Entities.AssessmentEntities
{
    public class AssessmentMatrixElement
    {
        [Key]
        public int Id { get; set; } // id элемента матрицы качественной оценки

        [Required]
        public int Row { get; set; } // номер строки текущего элемента

        [Required]
        public int Column { get; set; } // номер столбца текущего элемента

        [Required]
        public string Value { get; set; } // значение текущего элемента



        public int MatrixId { get; set; } // матрица, к которой относится текущий элемент
        public AssessmentMatrix Matrix { get; set; }



        public DateOnly DateOfCreation { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    }
}