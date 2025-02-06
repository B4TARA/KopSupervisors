using KOP.Common.Dtos.GradeDtos;
using KOP.Common.Enums;
using KOP.Common.Interfaces;

namespace KOP.BLL.Interfaces
{
    public interface IGradeService
    {
        Task<IBaseResponse<GradeDto>> GetGrade(int gradeId, List<GradeEntities> gradeEntitiesList);
        Task<IBaseResponse<object>> EditGrade(GradeDto dto);
    }
}