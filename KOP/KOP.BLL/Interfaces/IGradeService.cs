using KOP.Common.DTOs.GradeDTOs;
using KOP.Common.Enums;
using KOP.Common.Interfaces;

namespace KOP.BLL.Interfaces
{
    public interface IGradeService
    {
        Task<IBaseResponse<GradeDTO>> GetGrade(int gradeId, List<GradeEntities> gradeEntitiesList);
        Task<IBaseResponse<object>> EditGrade(GradeDTO dto);
    }
}