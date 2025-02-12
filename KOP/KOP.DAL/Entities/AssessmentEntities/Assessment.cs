using KOP.Common.Enums;

namespace KOP.DAL.Entities.AssessmentEntities
{
    public class Assessment
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public SystemStatuses SystemStatus { get; set; }

        public User User { get; set; }
        public int UserId { get; set; }

        public AssessmentType AssessmentType { get; set; }
        public int AssessmentTypeId { get; set; }

        public List<AssessmentResult> AssessmentResults { get; set; } = new();

        public DateOnly DateOfCreation { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    }
}