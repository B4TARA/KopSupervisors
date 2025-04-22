using KOP.Common.Enums;

namespace KOP.DAL.Entities
{
    public class Assessment
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public SystemStatuses SystemStatus { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int AssessmentTypeId { get; set; }
        public AssessmentType AssessmentType { get; set; }

        public int GradeId { get; set; }
        public Grade Grade { get; set; }

        public List<AssessmentResult> AssessmentResults { get; set; } = new();

        public DateOnly DateOfCreation { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    }
}