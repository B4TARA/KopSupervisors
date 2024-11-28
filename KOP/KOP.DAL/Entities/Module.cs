using System.ComponentModel.DataAnnotations;

namespace KOP.DAL.Entities
{
    public class Module
    {
        [Key]
        public int Id { get; set; } // id модуля

        [Required]
        public string Name { get; set; } // Название модуля  



        public ModuleType ModuleType { get; set; } // Тип, к которому относится данный модуль
        public int ModuleTypeId { get; set; }



        public virtual Module Parent { get; set; } // Родительский модуль
        public int? ParentId { get; set; }



        public virtual List<Module> Children { get; set; } = new(); // Дочерние модули
        public List<Employee> Employees { get; set; } = new(); // Сотрудники модуля



        public DateOnly DateOfCreation { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    }
}