using KOP.Common.Dtos.GradeDtos;

namespace KOP.BLL.Interfaces
{
    public interface IValueJudgmentService
    {
        Task<ValueJudgmentDto> GetValueJudgmentDtoByGradeId(int gradeId);
        Task EditValueJudgment(ValueJudgmentDto valueJudgmentDto);
    }
}