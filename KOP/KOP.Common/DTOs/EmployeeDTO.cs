using KOP.Common.DTOs.GradeDTOs;

namespace KOP.Common.DTOs
{
    public class EmployeeDTO
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Position { get; set; }
        public string Subdivision { get; set; }
        public string GradeGroup { get; set; }
        public string WorkPeriod { get; set; }
        public DateOnly ContractEndDate { get; set; }
        public string ImagePath { get; set; }
        public GradeDTO? LastGrade { get; set; }
    }
}