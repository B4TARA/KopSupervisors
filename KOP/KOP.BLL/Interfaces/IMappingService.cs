using KOP.Common.Dtos.AssessmentDtos;
using KOP.Common.Dtos.AssessmentResultDtos;
using KOP.Common.Dtos.GradeDtos;
using KOP.DAL.Entities;

namespace KOP.BLL.Interfaces
{
    public interface IMappingService
    {
        GradeExtendedDto CreateGradeDto(Grade grade, IEnumerable<MarkType> allMarkTypes);
        MarkDto CreateMarkDto(Mark mark);
        KpiDto CreateKpiDto(Kpi kpi);
        ProjectDto CreateProjectDto(Project project);
        StrategicTaskDto CreateStrategicTaskDto(StrategicTask strategicTask);
        TrainingEventDto CreateTrainingEventDto(TrainingEvent trainingEvent);
        AssessmentDto CreateAssessmentDto(Assessment assessment);
        AssessmentResultDto CreateAssessmentResultDto(AssessmentResult result, AssessmentType type);
        AssessmentResultValueDto CreateAssessmentResultValueDto(AssessmentResultValue value);
        AssessmentMatrixElementDto CreateAssessmentMatrixElementDto(AssessmentMatrixElement element);
        AssessmentInterpretationDto CreateAssessmentInterpretationDto(AssessmentInterpretation interpretation);
    }
}