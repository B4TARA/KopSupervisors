using KOP.Common.Dtos.GradeDtos;
using KOP.Common.Enums;

namespace KOP.Common.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Position { get; set; }
        public string SubdivisionFromFile { get; set; }
        public string GradeGroup { get; set; }
        public string WorkPeriod { get; set; }
        public string NextGradeStartDate { get; set; }
        public DateOnly ContractEndDate { get; set; }
        public string ImagePath { get; set; }
        public List<SystemRoles> SystemRoles { get; set; } = new();
        public GradeDto? LastGrade { get; set; }
        public bool PendingGradeStatus { get; set; }
    }
}