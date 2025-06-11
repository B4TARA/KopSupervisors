using KOP.Common.Dtos.GradeDtos;
using KOP.Common.Enums;
using KOP.DAL.Entities;

namespace KOP.BLL.Interfaces
{
    public interface IGradeService
    {
        Task<GradeExtendedDto> GetGradeDto(int gradeId, IEnumerable<GradeEntities> gradeEntities);
        Task EditGrade(GradeExtendedDto dto);
        Task DeleteStrategicTask(int id);
        Task DeleteProject(int id);
        Task DeleteKpi(int id);
        Task DeleteMark(int id);
        Task<GradeDto?> GetLatestGradeForUser(int userId);
        int CalculateCompletedCriteriaCount(Grade grade);
    }
}