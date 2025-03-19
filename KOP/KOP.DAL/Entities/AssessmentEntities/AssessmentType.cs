using KOP.Common.Enums;

namespace KOP.DAL.Entities.AssessmentEntities
{
    public class AssessmentType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public SystemAssessmentTypes SystemAssessmentType { get; set; }

        public AssessmentMatrix AssessmentMatrix { get; set; }
        public int AssessmentMatrixId { get; set; }

        public List<Assessment> Assessments { get; set; } = new();
        public List<AssessmentInterpretation> AssessmentInterpretations { get; set; } = new();
    }
}