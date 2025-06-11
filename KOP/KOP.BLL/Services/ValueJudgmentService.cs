using KOP.BLL.Interfaces;
using KOP.Common.Dtos.GradeDtos;
using KOP.DAL;
using Microsoft.EntityFrameworkCore;

namespace KOP.BLL.Services
{
    public class ValueJudgmentService : IValueJudgmentService
    {
        private readonly ApplicationDbContext _context;

        public ValueJudgmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ValueJudgmentDto> GetValueJudgmentForGrade(int gradeId)
        {
            var valueJudgment = await _context.ValueJudgments
                .AsNoTracking()
                .Select(vj => new ValueJudgmentDto
                {
                    Id = vj.Id,
                    GradeId = vj.GradeId,
                    Strengths = vj.Strengths,
                    BehaviorToCorrect = vj.BehaviorToCorrect,
                    RecommendationsForDevelopment = vj.RecommendationsForDevelopment,
                    IsFinalized = vj.Grade.IsValueJudgmentFinalized,
                })
                .FirstOrDefaultAsync(vj => vj.GradeId == gradeId);

            if (valueJudgment == null)
                throw new KeyNotFoundException($"ValueJudgment with Grade ID {gradeId} not found.");

            return valueJudgment;
        }

        public async Task EditValueJudgment(ValueJudgmentDto valueJudgmentDto)
        {
            var valueJudgment = await _context.ValueJudgments
                .Include(vj => vj.Grade)
                .FirstOrDefaultAsync(vj => vj.Id == valueJudgmentDto.Id);

            if (valueJudgment == null)
                throw new KeyNotFoundException($"ValueJudgment with ID {valueJudgmentDto.Id} not found.");

            valueJudgment.Strengths = valueJudgmentDto.Strengths ?? string.Empty;
            valueJudgment.BehaviorToCorrect = valueJudgmentDto.BehaviorToCorrect ?? string.Empty;
            valueJudgment.RecommendationsForDevelopment = valueJudgmentDto.RecommendationsForDevelopment ?? string.Empty;
            valueJudgment.Grade.IsValueJudgmentFinalized = valueJudgmentDto.IsFinalized;

            await _context.SaveChangesAsync();
        }
    }
}