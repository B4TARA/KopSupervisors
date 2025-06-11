using KOP.Common.Enums;

namespace KOP.DAL.Entities
{
    public class Recommendation : BaseEntity
    {
        protected Recommendation()
        {
            Value = string.Empty;
        }

        public Recommendation(string value, RecommendationTypes type, int gradeId)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value cannot be empty", nameof(value));
            if (gradeId <= 0)
                throw new ArgumentException("GradeId must be positive", nameof(value));
            if (!Enum.IsDefined(typeof(RecommendationTypes), type))
                throw new ArgumentException("Invalid recommendation type", nameof(type));

            Value = value;
            Type = type;
            GradeId = gradeId;
            DateOfCreation = DateTime.UtcNow;
            DateOfModification = DateTime.UtcNow;
        }
        public string Value { get; set; }
        public RecommendationTypes Type { get; set; }

        public int GradeId { get; set; }
        public Grade Grade { get; set; } = null!;
    }
}