using KOP.DAL.Interfaces.AssessmentInterfaces;
using KOP.DAL.Interfaces.GradeInterfaces;

namespace KOP.DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IAssessmentMatrixElementRepository AssessmentMatrixElements { get; }
        IAssessmentMatrixRepository AssessmentMatrices { get; }
        IAssessmentRepository Assessments { get; }
        IAssessmentResultRepository AssessmentResults { get; }
        IAssessmentResultValueRepository AssessmentResultValues { get; }
        IAssessmentTypeRepository AssessmentTypes { get; }
        IGradeRepository Grades { get; }
        IKpiRepository Kpis { get; }
        IPreviousJobRepository PreviousJobs { get; }
        IProjectRepository Projects { get; }
        IEmployeeRepository Employees { get; }
        IQualificationRepository Qualifications { get; }
        IMarkRepository Marks { get; }
        IMarkTypeRepository MarkTypes { get; }
        IModuleTypeRepository ModuleTypes { get; }
        IModuleRepository Modules { get; }
        IStrategicTaskRepository StrategicTasks { get; }
        ITrainingEventRepository TrainingEvents { get; }
        IValueJudgmentRepository ValueJudgments { get; }
        IMailRepository Mails { get; }

        void Commit();
        void Rollback();
        Task CommitAsync();
        Task RollbackAsync();
    }
}