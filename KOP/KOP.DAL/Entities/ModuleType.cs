using System.ComponentModel.DataAnnotations;

namespace KOP.DAL.Entities
{
    public class ModuleType
    {
        [Key]
        public int Id { get; set; } // id типа модуля

        [Required]
        public string Name { get; set; } // Название типа модуля  



        public List<Module> Modules { get; set; } = new(); // Модули, относящиеся к данному типу



        public DateOnly DateOfCreation { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    }
}