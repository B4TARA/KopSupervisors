using KOP.DAL.Entities;
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

        public DbSet<AssessmentInterpretation> AssessmentInterpretations { get; set; }
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
        public DbSet<User> Users { get; set; }
        public DbSet<Qualification> Qualifications { get; set; }
        public DbSet<Mark> Marks { get; set; }
        public DbSet<Mail> Mails { get; set; }
        public DbSet<MarkType> MarkTypes { get; set; }
        public DbSet<Subdivision> Subdivisions { get; set; }
        public DbSet<StrategicTask> StrategicTasks { get; set; }
        public DbSet<TrainingEvent> TrainingEvents { get; set; }
        public DbSet<ValueJudgment> ValueJudgments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Assessment>()
                .HasIndex(a => new { a.AssessmentTypeId, a.GradeId })
                .IsUnique();

            modelBuilder.Entity<Qualification>()
                .HasOne(a => a.Grade)
                .WithOne(a => a.Qualification)
                .HasForeignKey<Grade>(c => c.QualificationId);

            modelBuilder.Entity<Grade>()
                .HasOne(g => g.Qualification)
                .WithOne(q => q.Grade)
                .HasForeignKey<Grade>(g => g.QualificationId);

            modelBuilder.Entity<ValueJudgment>()
                .HasOne(a => a.Grade)
                .WithOne(a => a.ValueJudgment)
                .HasForeignKey<Grade>(c => c.ValueJudgmentId);

            modelBuilder.Entity<User>()
                .HasOne(u => u.ParentSubdivision)
                .WithMany(s => s.Users)
                .HasForeignKey(u => u.ParentSubdivisionId);

            modelBuilder.Entity<User>()
                .HasMany(u => u.SubordinateSubdivisions)
                .WithMany(u => u.SupervisingUsers);
        }
    }
}