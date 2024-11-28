using System.ComponentModel.DataAnnotations;

namespace KOP.DAL.Entities.GradeEntities
{
    public class MarkType
    {
        [Key]
        public int Id { get; set; } // Id типа показателя

        [Required]
        public string Name { get; set; } // Название типа показателя

        [Required]
        public string Description { get; set; } // Описание типа показателя



        public List<Mark> Marks { get; set; } = new(); // Список показателей, относящихся к данному типу



        public DateOnly DateOfCreation { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    }
}