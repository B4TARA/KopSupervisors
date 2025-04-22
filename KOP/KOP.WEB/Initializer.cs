using KOP.BLL.Interfaces;
using KOP.BLL.Services;
using KOP.DAL.Interfaces;
using KOP.DAL.Repositories;
using KOP.EmailService;

namespace KOP.WEB
{
    public static class Initializer
    {
        public static void InitializeRepositories(this IServiceCollection services)
        {
            services.AddScoped<IAssessmentInterpretationRepository, AssessmentInterpretationRepository>();
            services.AddScoped<IAssessmentRangeRepository, AssessmentRangeRepository>();
            services.AddScoped<IAssessmentMatrixElementRepository, AssessmentMatrixElementRepository>();
            services.AddScoped<IAssessmentMatrixRepository, AssessmentMatrixRepository>();
            services.AddScoped<IAssessmentRepository, AssessmentRepository>();
            services.AddScoped<IAssessmentResultRepository, AssessmentResultRepository>();
            services.AddScoped<IAssessmentTypeRepository, AssessmentTypeRepository>();
            services.AddScoped<IGradeRepository, GradeRepository>();
            services.AddScoped<IKpiRepository, KpiRepository>();
            services.AddScoped<IPreviousJobRepository, PreviousJobRepository>();
            services.AddScoped<IHigherEducationRepository, HigherEducationRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IQualificationRepository, QualificationRepository>();
            services.AddScoped<IMarkRepository, MarkRepository>();
            services.AddScoped<IMarkTypeRepository, MarkTypeRepository>();
            services.AddScoped<ISubdivisionRepository, SubdivisionRepository>();
            services.AddScoped<IStrategicTaskRepository, StrategicTaskRepository>();
            services.AddScoped<ITrainingEventRepository, TrainingEventRepository>();
            services.AddScoped<IValueJudgmentRepository, ValueJudgmentRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        public static void InitializeServices(this IServiceCollection services)
        {
            services.AddScoped<IValueJudgmentService, ValueJudgmentService>();
            services.AddScoped<ICommonService, CommonService>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IGradeService, GradeService>();
            services.AddScoped<IMappingService, MappingService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ISupervisorService, SupervisorService>();
            services.AddScoped<IAssessmentService, AssessmentService>();
            services.AddScoped<IAssessmentResultService, AssessmentResultService>();
        }
    }
}