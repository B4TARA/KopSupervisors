using System.ComponentModel.DataAnnotations;

namespace KOP.DAL.Entities.GradeEntities
{
    public class Mark
    {
        [Key]
        public int Id { get; set; } // Id показателя

        [Required]
        public int PercentageValue { get; set; } // Процентное значение показателя

        [Required]
        public string Period { get; set; } // Период показателя



        public MarkType MarkType { get; set; } // Тип показателя, к которому относится данный показатель
        public int MarkTypeId { get; set; }



        public Grade Grade { get; set; } // Оценка, к которой относится данный показатель
        public int GradeId { get; set; }



        public DateOnly DateOfCreation { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    }
}