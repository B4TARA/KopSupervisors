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
        IBaseResponse<EmployeeDTO> CreateEmployeeDTO(Employee employee);
        IBaseResponse<GradeDTO> CreateGradeDTO(Grade grade);
        IBaseResponse<QualificationDTO> CreateQualificationDTO(Qualification qualification);
        IBaseResponse<MarkDTO> CreateMarkDTO(Mark mark);
        IBaseResponse<KpiDTO> CreateKpiDTO(Kpi kpi);
        IBaseResponse<ProjectDTO> CreateProjectDTO(Project project);
        IBaseResponse<StrategicTaskDTO> CreateStrategicTaskDTO(StrategicTask strategicTask);
        IBaseResponse<ValueJudgmentDTO> CreateValueJudgmentDTO(ValueJudgment valueJudgment);
        IBaseResponse<PreviousJobDTO> CreatePreviousJobDTO(PreviousJob previousJob);
        IBaseResponse<TrainingEventDTO> CreateTrainingEventDTO(TrainingEvent trainingEvent);
        IBaseResponse<AssessmentDTO> CreateAssessmentDTO(Assessment assessment);
        IBaseResponse<AssessmentResultDTO> CreateAssessmentResultDTO(AssessmentResult result, AssessmentMatrix matrix);
        IBaseResponse<AssessmentResultValueDTO> CreateAssessmentResultValueDTO(AssessmentResultValue value);
        IBaseResponse<AssessmentMatrixElementDTO> CreateAssessmentMatrixElementDTO(AssessmentMatrixElement element);
    }
}