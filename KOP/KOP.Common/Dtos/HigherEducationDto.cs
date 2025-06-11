namespace KOP.Common.Dtos.GradeDtos
{
    public class HigherEducationDto
    {
        public int Id { get; set; }
        public string Education { get; set; }
        public string Speciality { get; set; }
        public string QualificationName { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
    }
}