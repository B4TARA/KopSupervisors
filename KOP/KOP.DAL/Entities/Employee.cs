using KOP.Common.Enums;
using KOP.DAL.Entities.AssessmentEntities;
using KOP.DAL.Entities.GradeEntities;
using System.ComponentModel.DataAnnotations;

namespace KOP.DAL.Entities
{
    public class Employee
    {
        [Key]
        public int Id { get; set; } // Id сотрудника

        [Required]
        public string FullName { get; set; } // ФИО сотрудника

        [Required]
        public string Position { get; set; } // Должность сотрудника

        [Required]
        public string Subdivision { get; set; } // Подразделение сотрудника

        [Required]
        public string GradeGroup { get; set; } // Группа грейда сотрудника

        [Required]
        public string WorkPeriod { get; set; } // Срок работы в банке

        [Required]
        public DateOnly ContractStartDate { get; set; } // Дата начала контракта

        [Required]
        public DateOnly ContractEndDate { get; set; } // Дата окончания контракта



        [Required]
        public string Login { get; set; } // Логин сотрудника для входа в систему

        [Required]
        public string Password { get; set; } // Пароль сотрудника для входа в систему

        [Required]
        public string Email { get; set; } // Рабочая почта сотрудника (для рассылки уведомлений и восстановления пароля)

        [Required]
        public string ImagePath { get; set; } = string.Empty; // Путь к аватарке сотрудника

        [Required]
        public bool IsSuspended { get; set; } // Флаг ограничения доступа сотрудника ко входу в систему



        public Module Module { get; set; } // Модуль, к которому относится данный сотрудник
        public int ModuleId { get; set; }



        public List<SystemRoles> SystemRoles { get; set; } = new(); // Роли сотрудника
        public List<Kpi> Kpis { get; set; } = new(); // Показатели КПЭ сотрудника
        public List<Mark> Marks { get; set; } = new(); // Показатели сотрудника
        public List<Project> Projects { get; set; } = new(); // Проекты
        public List<TrainingEvent> TrainingEvents { get; set; } = new(); // Обучающие мероприятия
        public List<StrategicTask> StrategicTasks { get; set; } = new(); // Задачи
        public List<Grade> Grades { get; set; } = new(); // Оценки карьерного роста сотрудника
        public List<Assessment> Assessments { get; set; } = new(); // качественные оценки сотрудника



        public DateOnly DateOfCreation { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    }
}