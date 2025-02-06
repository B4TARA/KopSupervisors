using KOP.Common.Dtos;
using KOP.Common.Dtos.AssessmentDtos;
using KOP.Common.Dtos.GradeDtos;
using KOP.Common.Interfaces;
using KOP.DAL.Entities;
using KOP.DAL.Entities.AssessmentEntities;
using KOP.DAL.Entities.GradeEntities;

namespace KOP.BLL.Interfaces
{
    public interface IMappingService
    {
        IBaseResponse<UserDto> CreateUserDto(User user);
        IBaseResponse<GradeDto> CreateGradeDto(Grade grade);
        IBaseResponse<QualificationDto> CreateQualificationDto(Qualification qualification);
        IBaseResponse<MarkDto> CreateMarkDto(Mark mark);
        IBaseResponse<KpiDto> CreateKpiDto(Kpi kpi);
        IBaseResponse<ProjectDto> CreateProjectDto(Project project);
        IBaseResponse<StrategicTaskDto> CreateStrategicTaskDto(StrategicTask strategicTask);
        IBaseResponse<ValueJudgmentDto> CreateValueJudgmentDto(ValueJudgment valueJudgment);
        IBaseResponse<PreviousJobDto> CreatePreviousJobDto(PreviousJob previousJob);
        IBaseResponse<TrainingEventDto> CreateTrainingEventDto(TrainingEvent trainingEvent);
        IBaseResponse<AssessmentDto> CreateAssessmentDto(Assessment assessment);
        IBaseResponse<AssessmentResultDto> CreateAssessmentResultDto(AssessmentResult result, AssessmentMatrix matrix);
        IBaseResponse<AssessmentResultValueDto> CreateAssessmentResultValueDto(AssessmentResultValue value);
        IBaseResponse<AssessmentMatrixElementDto> CreateAssessmentMatrixElementDto(AssessmentMatrixElement element);
    }
}