using System.ComponentModel.DataAnnotations;

namespace KOP.DAL.Entities.AssessmentEntities
{
    public class AssessmentType
    {
        [Key]
        public int Id { get; set; } // id типа качественной оценки

        [Required]
        public string Name { get; set; } // название типа качественной оценки

        public AssessmentMatrix AssessmentMatrix { get; set; } // матрица для данного типа оценки 
        public int AssessmentMatrixId { get; set; }

        public List<Assessment> Assessments { get; set; } = new();

        public DateOnly DateOfCreation { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    }
}