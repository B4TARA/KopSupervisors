using KOP.Common.Dtos;
using KOP.Common.Dtos.AssessmentDtos;
using KOP.Common.Dtos.GradeDtos;
using KOP.DAL.Entities;

namespace KOP.BLL.Interfaces
{
    public interface IMappingService
    {
        UserDto CreateUserDto(User user);
        GradeDto CreateGradeDto(Grade grade, IEnumerable<MarkType> allMarkTypes);
        QualificationDto CreateQualificationDto(Qualification qualification);
        MarkDto CreateMarkDto(Mark mark);
        KpiDto CreateKpiDto(Kpi kpi);
        ProjectDto CreateProjectDto(Project project);
        StrategicTaskDto CreateStrategicTaskDto(StrategicTask strategicTask);
        ValueJudgmentDto CreateValueJudgmentDto(ValueJudgment valueJudgment);
        PreviousJobDto CreatePreviousJobDto(PreviousJob previousJob);
        HigherEducationDto CreateHigherEducationDto(HigherEducation higherEducation);
        TrainingEventDto CreateTrainingEventDto(TrainingEvent trainingEvent);
        AssessmentDto CreateAssessmentDto(Assessment assessment);
        AssessmentResultDto CreateAssessmentResultDto(AssessmentResult result, AssessmentType type);
        AssessmentResultValueDto CreateAssessmentResultValueDto(AssessmentResultValue value);
        AssessmentMatrixElementDto CreateAssessmentMatrixElementDto(AssessmentMatrixElement element);
        AssessmentInterpretationDto CreateAssessmentInterpretationDto(AssessmentInterpretation interpretation);
    }
}