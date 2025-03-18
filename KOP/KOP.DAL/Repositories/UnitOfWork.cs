using KOP.DAL.Interfaces;
using KOP.DAL.Interfaces.AssessmentInterfaces;
using KOP.DAL.Interfaces.GradeInterfaces;
using KOP.DAL.Repositories.AssessmentRepositories;
using KOP.DAL.Repositories.GradeRepositories;

namespace KOP.DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;

        private IAssessmentInterpretationRepository? assessmentInterpretationRepository;
        private IAssessmentMatrixElementRepository? assessmentMatrixElementRepository;
        private IAssessmentMatrixRepository? assessmentMatrixRepository;
        private IAssessmentRepository? assessmentRepository;
        private IAssessmentResultRepository? assessmentResultRepository;
        private IAssessmentResultValueRepository? assessmentResultValueRepository;
        private IAssessmentTypeRepository? assessmentTypeRepository;
        private IGradeRepository? gradeRepository;
        private IPreviousJobRepository? previousJobRepository;
        private IKpiRepository? kpiRepository;
        private IProjectRepository? projectRepository;
        private IHigherEducationRepository? higherEducationRepository;
        private IUserRepository? userRepository;
        private IQualificationRepository? qualificationRepository;
        private IMarkRepository? markRepository;
        private IMarkTypeRepository? markTypeRepository;
        private ISubdivisionRepository? subdivisionRepository;
        private IStrategicTaskRepository? strategicTaskRepository;
        private ITrainingEventRepository? trainingEventRepository;
        private IValueJudgmentRepository? valueJudgmentRepository;
        private IMailRepository? mailRepository;

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IAssessmentInterpretationRepository AssessmentInterpretations
        {
            get
            {
                if (assessmentInterpretationRepository == null)
                    assessmentInterpretationRepository = new AssessmentInterpretationRepository(_dbContext);
                return assessmentInterpretationRepository;
            }
        }

        public IAssessmentMatrixElementRepository AssessmentMatrixElements
        {
            get
            {
                if (assessmentMatrixElementRepository == null)
                    assessmentMatrixElementRepository = new AssessmentMatrixElementRepository(_dbContext);
                return assessmentMatrixElementRepository;
            }
        }

        public IAssessmentMatrixRepository AssessmentMatrices
        {
            get
            {
                if (assessmentMatrixRepository == null)
                    assessmentMatrixRepository = new AssessmentMatrixRepository(_dbContext);
                return assessmentMatrixRepository;
            }
        }

        public IAssessmentRepository Assessments
        {
            get
            {
                if (assessmentRepository == null)
                    assessmentRepository = new AssessmentRepository(_dbContext);
                return assessmentRepository;
            }
        }

        public IAssessmentResultRepository AssessmentResults
        {
            get
            {
                if (assessmentResultRepository == null)
                    assessmentResultRepository = new AssessmentResultRepository(_dbContext);
                return assessmentResultRepository;
            }
        }

        public IAssessmentResultValueRepository AssessmentResultValues
        {
            get
            {
                if (assessmentResultValueRepository == null)
                    assessmentResultValueRepository = new AssessmentResultValueRepository(_dbContext);
                return assessmentResultValueRepository;
            }
        }

        public IAssessmentTypeRepository AssessmentTypes
        {
            get
            {
                if (assessmentTypeRepository == null)
                    assessmentTypeRepository = new AssessmentTypeRepository(_dbContext);
                return assessmentTypeRepository;
            }
        }

        public IGradeRepository Grades
        {
            get
            {
                if (gradeRepository == null)
                    gradeRepository = new GradeRepository(_dbContext);
                return gradeRepository;
            }
        }

        public IKpiRepository Kpis
        {
            get
            {
                if (kpiRepository == null)
                    kpiRepository = new KpiRepository(_dbContext);
                return kpiRepository;
            }
        }

        public IPreviousJobRepository PreviousJobs
        {
            get
            {
                if (previousJobRepository == null)
                    previousJobRepository = new PreviousJobRepository(_dbContext);
                return previousJobRepository;
            }
        }

        public IHigherEducationRepository HigherEducations
        {
            get
            {
                if (higherEducationRepository == null)
                    higherEducationRepository = new HigherEducationRepository(_dbContext);
                return higherEducationRepository;
            }
        }

        public IProjectRepository Projects
        {
            get
            {
                if (projectRepository == null)
                    projectRepository = new ProjectRepository(_dbContext);
                return projectRepository;
            }
        }

        public IUserRepository Users
        {
            get
            {
                if (userRepository == null)
                    userRepository = new UserRepository(_dbContext);
                return userRepository;
            }
        }

        public IQualificationRepository Qualifications
        {
            get
            {
                if (qualificationRepository == null)
                    qualificationRepository = new QualificationRepository(_dbContext);
                return qualificationRepository;
            }
        }

        public IMarkRepository Marks
        {
            get
            {
                if (markRepository == null)
                    markRepository = new MarkRepository(_dbContext);
                return markRepository;
            }
        }

        public IMarkTypeRepository MarkTypes
        {
            get
            {
                if (markTypeRepository == null)
                    markTypeRepository = new MarkTypeRepository(_dbContext);
                return markTypeRepository;
            }
        }

        public ISubdivisionRepository Subdivisions
        {
            get
            {
                if (subdivisionRepository == null)
                    subdivisionRepository = new SubdivisionRepository(_dbContext);
                return subdivisionRepository;
            }
        }

        public IStrategicTaskRepository StrategicTasks
        {
            get
            {
                if (strategicTaskRepository == null)
                    strategicTaskRepository = new StrategicTaskRepository(_dbContext);
                return strategicTaskRepository;
            }
        }

        public ITrainingEventRepository TrainingEvents
        {
            get
            {
                if (trainingEventRepository == null)
                    trainingEventRepository = new TrainingEventRepository(_dbContext);
                return trainingEventRepository;
            }
        }

        public IValueJudgmentRepository ValueJudgments
        {
            get
            {
                if (valueJudgmentRepository == null)
                    valueJudgmentRepository = new ValueJudgmentRepository(_dbContext);
                return valueJudgmentRepository;
            }
        }

        public IMailRepository Mails
        {
            get
            {
                if (mailRepository == null)
                    mailRepository = new MailRepository(_dbContext);
                return mailRepository;
            }
        }



        public void Commit()
             => _dbContext.SaveChanges();
        public async Task CommitAsync()
            => await _dbContext.SaveChangesAsync();
        public void Rollback()
            => _dbContext.Dispose();


        public async Task RollbackAsync()
            => await _dbContext.DisposeAsync();


        private bool disposed = false;


        public virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }

                disposed = true;
            }
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}