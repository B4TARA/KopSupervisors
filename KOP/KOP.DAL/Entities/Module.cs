namespace KOP.DAL.Entities
{
    public class Module
    {
        public int Id { get; set; } // id модуля
        public string? Name { get; set; } // Название модуля  

        public virtual Module? Parent { get; set; } // Родительский модуль
        public int? ParentId { get; set; }

        public virtual List<Module> Children { get; set; } = new(); // Дочерние модули
        public List<Employee> Employees { get; set; } = new(); // Сотрудники модуля

        public DateOnly DateOfCreation { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    }
}
