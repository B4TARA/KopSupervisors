using KOP.Common.DTOs;
using KOP.Common.DTOs.AssessmentDTOs;
using KOP.Common.DTOs.GradeDTOs;
using KOP.Common.Interfaces;
using KOP.DAL.Entities;
using KOP.DAL.Entities.AssessmentEntities;
using KOP.DAL.Entities.GradeEntities;

namespace KOP.BLL.Interfaces
{
    public interface IMappingService
    {
        IBaseResponse<EmployeeDTO> CreateUserDto(User user);
        IBaseResponse<GradeDTO> CreateGradeDto(Grade grade);
        IBaseResponse<QualificationDTO> CreateQualificationDto(Qualification qualification);
        IBaseResponse<MarkDTO> CreateMarkDto(Mark mark);
        IBaseResponse<KpiDTO> CreateKpiDto(Kpi kpi);
        IBaseResponse<ProjectDTO> CreateProjectDto(Project project);
        IBaseResponse<StrategicTaskDTO> CreateStrategicTaskDto(StrategicTask strategicTask);
        IBaseResponse<ValueJudgmentDTO> CreateValueJudgmentDto(ValueJudgment valueJudgment);
        IBaseResponse<PreviousJobDTO> CreatePreviousJobDto(PreviousJob previousJob);
        IBaseResponse<TrainingEventDTO> CreateTrainingEventDto(TrainingEvent trainingEvent);
        IBaseResponse<AssessmentDTO> CreateAssessmentDto(Assessment assessment);
        IBaseResponse<AssessmentResultDTO> CreateAssessmentResultDto(AssessmentResult result, AssessmentMatrix matrix);
        IBaseResponse<AssessmentResultValueDTO> CreateAssessmentResultValueDto(AssessmentResultValue value);
        IBaseResponse<AssessmentMatrixElementDTO> CreateAssessmentMatrixElementDto(AssessmentMatrixElement element);
    }
}