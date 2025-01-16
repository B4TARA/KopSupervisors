using KOP.Common.Enums;
using KOP.DAL.Entities;
using KOP.DAL.Entities.AssessmentEntities;
using KOP.DAL.Entities.GradeEntities;
using Microsoft.EntityFrameworkCore;
using Mail = KOP.DAL.Entities.Mail;

namespace KOP.DAL
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            //Database.EnsureDeleted();
            //Database.EnsureCreated();
        }

        public DbSet<AssessmentMatrixElement> AssessmentMatrixElements { get; set; }
        public DbSet<AssessmentMatrix> AssessmentMatrices { get; set; }
        public DbSet<Assessment> Assessments { get; set; }
        public DbSet<AssessmentResult> AssessmentResults { get; set; }
        public DbSet<AssessmentResultValue> AssessmentResultValues { get; set; }
        public DbSet<AssessmentType> AssessmentTypes { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Kpi> Kpis { get; set; }
        public DbSet<PreviousJob> PreviousJobs { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Qualification> Qualifications { get; set; }
        public DbSet<Mark> Marks { get; set; }
        public DbSet<MarkType> MarkTypes { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<ModuleType> ModuleTypes { get; set; }
        public DbSet<StrategicTask> StrategicTasks { get; set; }
        public DbSet<TrainingEvent> TrainingEvents { get; set; }
        public DbSet<ValueJudgment> ValueJudgments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Mail>(builder =>
            {
                builder.HasData(new Mail[]
                {
                    new Mail
                    {
                        Id = 1,
                        Code = MailCodes.CreateAssessmentGroupAndAssessEmployeeNotification,
                        Title = "Уведомление",
                        Body = "<div>\r\n    Пришло время подвести <b>итоги реализации SMART-задач</b> и заполнить <b>SMART-задачи на следующий месяц.</b>\r\n    <span>Вы можете сделать это по ссылке </span>\r\n    <a href=\"https://10.117.11.77:80/Account/LogOut\" target=\"_blank\">https://10.117.11.77:80/Account/LogOut</a>\r\n    <br>\r\n    <div>или через ярлык на рабочем столе</div>\r\n    <div>\r\n        <span style=\"width:50px; height:50px;\">\r\n            <img style=\"width:50px; height:50px;\" src='cid:{0}'>\r\n        </span>\r\n    </div>\r\n    <br>\r\n    <div>\r\n        Просим Вас указать оценочные суждения по задачам и заполнить SMART-задачи на следующий месяц.\r\n    </div>\r\n</div>\r\n",
                    },
                    new Mail
                    {
                        Id = 2,
                        Code = MailCodes.CreateAssessmentGroupAndAssessEmployeeReminder,
                        Title = "Напоминание",
                        Body = "<div>\r\n    Напоминаем о необходимости подвести <b>итоги реализации SMART-задач</b> и заполнить <b>SMART-задачи на следующий месяц.</b>\r\n    <span>Вы можете сделать это по ссылке </span>\r\n    <a href=\"https://10.117.11.77:80/Account/LogOut\" target=\"_blank\">https://10.117.11.77:80/Account/LogOut</a>\r\n    <br>\r\n    <div>или через ярлык на рабочем столе</div>\r\n    <div>\r\n        <span style=\"width:50px; height:50px;\">\r\n            <img style=\"width:50px; height:50px;\" src='cid:{0}'>\r\n        </span>\r\n    </div>\r\n    <br>\r\n    <div>\r\n        Просим Вас указать оценочные суждения по задачам и заполнить SMART-задачи на следующий месяц.\r\n    </div>\r\n</div>\r\n",
                    },
                    new Mail
                    {
                        Id = 3,
                        Code = MailCodes.CreatePerformanceResultsAndSelfAssessmentNotification,
                        Title = "Уведомление",
                        Body = "<div>\r\n    Пришло время подвести <b>итоги реализации SMART-задач Ваших работников</b> и согласовать <b>новые SMART-задачи.</b>\r\n    <span> Вы можете сделать это по ссылке </span>\r\n    <a href=\"https://10.117.11.77:80/Account/LogOut\" target=\"_blank\">https://10.117.11.77:80/Account/LogOut</a>\r\n    <br>\r\n    <div>или через ярлык на рабочем столе</div>\r\n    <div>\r\n        <span style=\"width:50px; height:50px;\">\r\n            <img style=\"width:50px; height:50px;\" src='cid:{0}'>\r\n        </span>\r\n    </div>\r\n    <br>\r\n    <div>\r\n        Просим Вас согласовать оценочные суждения по задачам Ваших работников и согласовать новые SMART-задачи\r\n    </div>\r\n</div>",
                    },
                    new Mail
                    {
                        Id = 4,
                        Code = MailCodes.CreatePerformanceResultsAndSelfAssessmentReminder,
                        Title = "Уведомление",
                        Body = "\"<div>\" +\r\n                \"Информируем, что Ваша задача отправлена на доработку и находится на этапе \\\"Составление SMART-задач работником.\\\"\" +\r\n                \"<br>\" +\r\n                \"Редактирование SMART-задач доступно по ссылке:\" +\r\n                    \"<a href= \\\"https://10.117.11.77:80/Account/LogOut\\\" target = \\\"blanc\\\">Посмотреть задачу можно по ссылке<a/>\" +\r\n                    \"<br>\" +\r\n                    \"<div>\"+\"или через ярлык на рабочем столе\"+\"<div>\" +\r\n                    \"<span style=\\\"width:50px; height:50px;\\\">\" +\r\n                            \"<img style=\\\"width:50px; height:50px;\\\" src='cid:{0}'>\" +\r\n                    \"</span>\" +\r\n            \"</div>\"",
                    },
                    new Mail
                    {
                        Id = 5,
                        Code = MailCodes.CreateCriteriaNotification,
                        Title = "Уведомление",
                        Body = "\"<div>\" +\r\n                \"Информируем, что Ваша задача отправлена на доработку и находится на этапе \\\"Составление SMART-задач работником.\\\"\" +\r\n                \"<br>\" +\r\n                \"Редактирование SMART-задач доступно по ссылке:\" +\r\n                    \"<a href= \\\"https://10.117.11.77:80/Account/LogOut\\\" target = \\\"blanc\\\">Посмотреть задачу можно по ссылке<a/>\" +\r\n                    \"<br>\" +\r\n                    \"<div>\"+\"или через ярлык на рабочем столе\"+\"<div>\" +\r\n                    \"<span style=\\\"width:50px; height:50px;\\\">\" +\r\n                            \"<img style=\\\"width:50px; height:50px;\\\" src='cid:{0}'>\" +\r\n                    \"</span>\" +\r\n            \"</div>\"",
                    },
                    new Mail
                    {
                        Id = 6,
                        Code = MailCodes.CreateCriteriaReminder,
                        Title = "Уведомление",
                        Body = "\"<div>\" +\r\n                \"Информируем, что Ваша задача отправлена на доработку и находится на этапе \\\"Составление SMART-задач работником.\\\"\" +\r\n                \"<br>\" +\r\n                \"Редактирование SMART-задач доступно по ссылке:\" +\r\n                    \"<a href= \\\"https://10.117.11.77:80/Account/LogOut\\\" target = \\\"blanc\\\">Посмотреть задачу можно по ссылке<a/>\" +\r\n                    \"<br>\" +\r\n                    \"<div>\"+\"или через ярлык на рабочем столе\"+\"<div>\" +\r\n                    \"<span style=\\\"width:50px; height:50px;\\\">\" +\r\n                            \"<img style=\\\"width:50px; height:50px;\\\" src='cid:{0}'>\" +\r\n                    \"</span>\" +\r\n            \"</div>\"",
                    },
                    new Mail
                    {
                        Id = 7,
                        Code = MailCodes.CreateAssessmentNotification,
                        Title = "Уведомление",
                        Body = "\"<div>\" +\r\n                \"Информируем, что Ваша задача отправлена на доработку и находится на этапе \\\"Составление SMART-задач работником.\\\"\" +\r\n                \"<br>\" +\r\n                \"Редактирование SMART-задач доступно по ссылке:\" +\r\n                    \"<a href= \\\"https://10.117.11.77:80/Account/LogOut\\\" target = \\\"blanc\\\">Посмотреть задачу можно по ссылке<a/>\" +\r\n                    \"<br>\" +\r\n                    \"<div>\"+\"или через ярлык на рабочем столе\"+\"<div>\" +\r\n                    \"<span style=\\\"width:50px; height:50px;\\\">\" +\r\n                            \"<img style=\\\"width:50px; height:50px;\\\" src='cid:{0}'>\" +\r\n                    \"</span>\" +\r\n            \"</div>\"",
                    },
                    new Mail
                    {
                        Id = 8,
                        Code = MailCodes.CreateAssessmentReminder,
                        Title = "Уведомление",
                        Body = "\"<div>\" +\r\n                \"Информируем, что Ваша задача отправлена на доработку и находится на этапе \\\"Составление SMART-задач работником.\\\"\" +\r\n                \"<br>\" +\r\n                \"Редактирование SMART-задач доступно по ссылке:\" +\r\n                    \"<a href= \\\"https://10.117.11.77:80/Account/LogOut\\\" target = \\\"blanc\\\">Посмотреть задачу можно по ссылке<a/>\" +\r\n                    \"<br>\" +\r\n                    \"<div>\"+\"или через ярлык на рабочем столе\"+\"<div>\" +\r\n                    \"<span style=\\\"width:50px; height:50px;\\\">\" +\r\n                            \"<img style=\\\"width:50px; height:50px;\\\" src='cid:{0}'>\" +\r\n                    \"</span>\" +\r\n            \"</div>\"",
                    },
                });
            });

            modelBuilder.Entity<ModuleType>(builder =>
            {
                builder.HasData(new ModuleType[]
                {
                    new ModuleType
                    {
                        Id = 1,
                        Name = "Направление",
                    },
                    new ModuleType
                    {
                        Id = 2,
                        Name = "ССП",
                    },
                });
            });

            modelBuilder.Entity<Qualification>()
            .HasOne(a => a.Grade)
            .WithOne(a => a.Qualification)
            .HasForeignKey<Grade>(c => c.QualificationId);

            modelBuilder.Entity<ValueJudgment>()
           .HasOne(a => a.Grade)
           .WithOne(a => a.ValueJudgment)
           .HasForeignKey<Grade>(c => c.ValueJudgmentId);
        }
    }
}