using System.ComponentModel.DataAnnotations;

namespace KOP.DAL.Entities.GradeEntities
{
    public class Qualification
    {
        [Key]
        public int Id { get; set; } // Id квалификации

        [Required]
        public string SupervisorSspName { get; set; } // ФИО руководителя ССП

        [Required]
        public string Link { get; set; } // Ссылка на ЛПА

        [Required]
        public string HigherEducation { get; set; } // Высшее Образование

        [Required]
        public string Speciality { get; set; } // Специальность

        [Required]
        public string QualificationResult { get; set; } // Квалификация

        [Required]
        public DateOnly StartDate { get; set; } // Период обучения "с"

        [Required]
        public DateOnly EndDate { get; set; } // Период обучения "по"

        [Required]
        public string AdditionalEducation { get; set; } // Дополнительное образование

        [Required]
        public string CurrentDate { get; set; } // Число для указания текущего стажа в банковской системе

        [Required]
        public string ExperienceMonths { get; set; } // Стаж в банковской системе месяцы

        [Required]
        public string ExperienceYears { get; set; } // Стаж в банковской системе лет

        [Required]
        public string PreviousPosition1 { get; set; } // Предыдущая должность 1

        [Required]
        public string PreviousPosition2 { get; set; } // Предыдущая должность 2

        [Required]
        public string CurrentPosition { get; set; } // Текущая должность

        [Required]
        public string EmploymentContarctTerminations { get; set; } // факты расторжения трудового договора



        public Employee Employee { get; set; } // Сотрудник, к которому относится данный стратегический проекта
        public int EmployeeId { get; set; }



        public Grade Grade { get; set; } // Оценка, к которой относится данный стратегический проекта
        public int GradeId { get; set; }



        public DateOnly DateOfCreation { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    }
}
