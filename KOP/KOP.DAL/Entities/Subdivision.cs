namespace KOP.DAL.Entities
{
    public class Subdivision
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int NestingLevel { get; set; } = 1;

        public virtual Subdivision? Parent { get; set; }
        public int? ParentId { get; set; }

        public virtual List<Subdivision> Children { get; set; } = new();
        public List<User> Users { get; set; } = new();
        public List<User> SupervisingUsers { get; set; } = new();
    }
}
