namespace KOP.DAL.Entities
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public DateOnly DateOfCreation { get; set; }
        public DateOnly DateOfModification { get; set; }
    }
}