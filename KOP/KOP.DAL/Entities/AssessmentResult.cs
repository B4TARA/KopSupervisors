using KOP.Common.Enums;

namespace KOP.DAL.Entities
{
    public class AssessmentResult
    {
        public int Id { get; set; }
        public DateOnly? ResultDate { get; set; }
        public SystemStatuses SystemStatus { get; set; }
        public AssessmentResultTypes Type { get; set; }

        public int? AssignedBy { get; set; }

        public Assessment Assessment { get; set; }
        public int AssessmentId { get; set; }

        public User Judge { get; set; }
        public int JudgeId { get; set; }

        public List<AssessmentResultValue> AssessmentResultValues { get; set; } = new();
        public int TotalValue => AssessmentResultValues.Sum(x => x.Value);

        public DateOnly DateOfCreation { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    }
}