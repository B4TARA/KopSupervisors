using KOP.BLL.Interfaces;
using KOP.BLL.Services;
using KOP.DAL.Interfaces;
using KOP.DAL.Interfaces.AssessmentInterfaces;
using KOP.DAL.Interfaces.GradeInterfaces;
using KOP.DAL.Repositories;
using KOP.DAL.Repositories.AssessmentRepositories;
using KOP.DAL.Repositories.GradeRepositories;

namespace KOP.WEB
{
    public static class Initializer
    {
        public static void InitializeRepositories(this IServiceCollection services)
        {
            services.AddScoped<IMailRepository, MailRepository>();
            services.AddScoped<IAssessmentMatrixElementRepository, AssessmentMatrixElementRepository>();
            services.AddScoped<IAssessmentMatrixRepository, AssessmentMatrixRepository>();
            services.AddScoped<IAssessmentRepository, AssessmentRepository>();
            services.AddScoped<IAssessmentResultRepository, AssessmentResultRepository>();
            services.AddScoped<IAssessmentTypeRepository, AssessmentTypeRepository>();
            services.AddScoped<IGradeRepository, GradeRepository>();
            services.AddScoped<IKpiRepository, KpiRepository>();
            services.AddScoped<IPreviousJobRepository, PreviousJobRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IQualificationRepository, QualificationRepository>();
            services.AddScoped<IMarkRepository, MarkRepository>();
            services.AddScoped<IMarkTypeRepository, MarkTypeRepository>();
            services.AddScoped<ISubdivisionRepository, SubdivisionRepository>();
            services.AddScoped<IModuleTypeRepository, ModuleTypeRepository>();
            services.AddScoped<IStrategicTaskRepository, StrategicTaskRepository>();
            services.AddScoped<ITrainingEventRepository, TrainingEventRepository>();
            services.AddScoped<IValueJudgmentRepository, ValueJudgmentRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        public static void InitializeServices(this IServiceCollection services)
        {
            services.AddScoped<IGradeService, GradeService>();
            services.AddScoped<IMappingService, MappingService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IEmployeeService, UserService>();
            services.AddScoped<ISupervisorService, SupervisorService>();
            services.AddScoped<IAssessmentService, AssessmentService>();
        }
    }
}