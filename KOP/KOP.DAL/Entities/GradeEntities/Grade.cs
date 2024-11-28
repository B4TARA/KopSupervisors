using System.ComponentModel.DataAnnotations;

namespace KOP.DAL.Entities.GradeEntities
{
    public class Grade
    {
        [Key]
        public int Id { get; set; } // Id оценки

        [Required]
        public int Number { get; set; } // Номер оценки (очередность)

        [Required]
        public DateOnly StartDate { get; set; } // Дата начала оценки

        [Required]
        public DateOnly EndDate { get; set; } // Дата завершения оценки

        public DateOnly? NextGradeDate { get; set; } // Дата следующей оценки

        public string? StrategicTasksConclusion { get; set; } // Вывод для подзадачника
        public string? KPIsConclusion { get; set; } // Вывод для КПЭ
        public string? QualificationConclusion { get; set; } // Вывод для Квалификации
        public string? ManagmentCompetenciesConclusion { get; set; } // Вывод для управленческих компетенций
        public string? CorporateCompetenciesConclusion { get; set; } // Вывод для корпоративных компетенций



        public Employee Employee { get; set; } // пользователь, к которому относится данная оценка
        public int EmployeeId { get; set; }



        public Qualification? Qualification { get; set; } // квалификация, которая относится к данной оценке
        public int? QualificationId { get; set; }



        public ValueJudgment? ValueJudgment { get; set; } // Квалификация, которая относится к данной оценке
        public int? ValueJudgmentId { get; set; }



        public List<Mark> Marks { get; set; } = new(); // Показатели
        public List<Kpi> Kpis { get; set; } = new(); // Показатели КПЭ
        public List<Project> Projects { get; set; } = new(); // Проекты
        public List<StrategicTask> StrategicTasks { get; set; } = new(); // Задачи
        public List<TrainingEvent> TrainingEvents { get; set; } = new(); // Обучающие мероприятия



        public DateOnly DateOfCreation { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    }
}