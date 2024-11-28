using KOP.Common.DTOs;
using KOP.Common.DTOs.GradeDTOs;
using KOP.Common.Enums;
using KOP.Common.Interfaces;

namespace KOP.BLL.Interfaces
{
    public interface IGradeService
    {
        Task<IBaseResponse<GradeDTO>> GetGrade(int gradeId, List<GradeEntities> gradeEntitiesList);
        Task<IBaseResponse<object>> CreateStrategicTask(StrategicTaskDTO dto);
        Task<IBaseResponse<object>> CreateProject(ProjectDTO dto);
        Task<IBaseResponse<object>> CreateKpi(KpiDTO dto);
        Task<IBaseResponse<object>> CreateMark(MarkDTO dto);
        Task<IBaseResponse<object>> CreateTrainingEvent(TrainingEventDTO dto);
        Task<IBaseResponse<object>> CreateQualification(QualificationDTO dto);
        Task<IBaseResponse<object>> EditValueJudgment(ValueJudgmentDTO dto);
    }
}