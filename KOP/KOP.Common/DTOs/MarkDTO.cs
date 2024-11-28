namespace KOP.Common.DTOs
{
    public class MarkDTO
    {
        public int Id { get; set; }
        public int PercentageValue { get; set; }
        public string Period { get; set; }
        public int EmployeeId { get; set; }
        public int GradeId { get; set; }
        public MarkTypeDTO MarkTypeDTO { get; set; }
    }
}