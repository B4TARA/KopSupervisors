namespace KOP.DAL.Entities
{
    public class HigherEducation
    {
        public int Id { get; set; }
        public string Education { get; set; }
        public string Speciality { get; set; }
        public string QualificationName { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }

        public Qualification Qualification { get; set; }
        public int QualificationId { get; set; }

        public DateOnly DateOfCreation { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    }
}