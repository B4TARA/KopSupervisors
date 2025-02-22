﻿using System.ComponentModel.DataAnnotations;

namespace KOP.DAL.Entities.GradeEntities
{
    public class Project
    {
        [Key]
        public int Id { get; set; } // id стратегического проекта

        [Required]
        public string Name { get; set; } // Наименование проекта

        [Required]
        public string SupervisorSspName { get; set; } // ФИО руководителя ССП

        [Required]
        public string Stage { get; set; } // Этап проекта

        [Required]
        public DateOnly StartDate { get; set; } // Дата открытия проекта

        [Required]
        public DateOnly EndDate { get; set; } // Срок реализации проекта

        [Required]
        public DateOnly CurrentStatusDate { get; set; } // Дата текущего состояния

        [Required]
        public int PlanStages { get; set; } // План этапов

        [Required]
        public int FactStages { get; set; } // Факт этапов

        [Required]
        public int SPn { get; set; } // Оценка реализации проекта



        public Grade Grade { get; set; } // Оценка, к которой относится данный стратегический проекта
        public int GradeId { get; set; }



        public DateOnly DateOfCreation { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    }
}