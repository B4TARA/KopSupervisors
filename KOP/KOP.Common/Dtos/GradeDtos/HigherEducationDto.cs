namespace KOP.Common.Dtos.GradeDtos
{
    public class HigherEducationDto
    {
        public int? Id { get; set; }
        public string Education { get; set; }
        public string Speciality { get; set; }
        public string QualificationName { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateOnly StartDate => DateOnly.FromDateTime(StartDateTime);
        public DateTime EndDateTime { get; set; }
        public DateOnly EndDate => DateOnly.FromDateTime(EndDateTime);
    }
}