using KOP.Common.Dtos.AssessmentDtos;
using KOP.Common.Dtos.GradeDtos;
using KOP.Common.Enums;
using KOP.Common.Interfaces;

namespace KOP.BLL.Interfaces
{
    public interface IGradeService
    {
        Task<IBaseResponse<GradeDto>> GetGrade(int gradeId, List<GradeEntities> gradeEntitiesList);
        Task<IBaseResponse<object>> EditGrade(GradeDto dto);
        Task<IBaseResponse<object>> DeleteStrategicTask(int id);
        Task<IBaseResponse<object>> DeleteProject(int id);
        Task<IBaseResponse<object>> DeleteKpi(int id);
        Task<IBaseResponse<object>> DeleteMark(int id);
        Task<IBaseResponse<object>> DeletePreviousJob(int id);
    }
}