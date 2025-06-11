using KOP.Common.Dtos.GradeDtos;

namespace KOP.Common.Dtos.UserDtos
{
    public class UserDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Position { get; set; }
        public string SubdivisionFromFile { get; set; }
        public string GradeGroup { get; set; }
        public string WorkPeriod { get; set; }
        public string ImagePath { get; set; }
        public string ContractEndDate { get; set; }
        public string NextGradeStartDate { get; set; }
        public GradeDto? LastGrade {  get; set; }
    }
}