﻿using KOP.DAL.Entities;
using KOP.DAL.Entities.AssessmentEntities;
using KOP.DAL.Entities.GradeEntities;
using Microsoft.EntityFrameworkCore;

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