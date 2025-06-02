using KOP.Common.Dtos.GradeDtos;

namespace KOP.BLL.Interfaces
{
    public interface IValueJudgmentService
    {
        Task<ValueJudgmentDto> GetValueJudgmentByGradeId(int gradeId);
        Task EditValueJudgment(ValueJudgmentDto valueJudgmentDto);
    }
}