using KOP.BLL.Interfaces;
using KOP.Common.Dtos.GradeDtos;
using KOP.DAL;
using Microsoft.EntityFrameworkCore;

namespace KOP.BLL.Services
{
    public class ValueJudgmentService : IValueJudgmentService
    {
        private readonly ApplicationDbContext _dbContext;

        public ValueJudgmentService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ValueJudgmentDto> GetValueJudgmentDtoByGradeId(int gradeId)
        {
            if (gradeId == 0)
                throw new ArgumentException("Grade ID cannot be 0", nameof(gradeId));

            var valueJudgmentDto = await _dbContext.ValueJudgments
                .AsNoTracking()
                .Select(x => new ValueJudgmentDto
                {
                    Id = x.Id,
                    GradeId = x.GradeId,
                    Strengths = x.Strengths,
                    BehaviorToCorrect = x.BehaviorToCorrect,
                    RecommendationsForDevelopment = x.RecommendationsForDevelopment,
                    IsFinalized = x.IsFinalized,
                })
                .SingleOrDefaultAsync(x => x.GradeId == gradeId);

            if (valueJudgmentDto == null)
                throw new KeyNotFoundException($"ValueJudgment with Grade ID {gradeId} not found.");

            return valueJudgmentDto;
        }

        public async Task EditValueJudgment(ValueJudgmentDto valueJudgmentDto)
        {
            if (valueJudgmentDto == null)
                throw new ArgumentNullException(nameof(valueJudgmentDto), "ValueJudgment cannot be null.");
            else if (valueJudgmentDto.Id == 0)
                throw new ArgumentException("ValueJudgment ID cannot be 0", nameof(valueJudgmentDto));

            var valueJudgment = await _dbContext.ValueJudgments
                .SingleOrDefaultAsync(x => x.Id == valueJudgmentDto.Id);

            if (valueJudgment == null)
                throw new KeyNotFoundException($"ValueJudgment with ID {valueJudgmentDto.Id} not found.");

            valueJudgment.Strengths = valueJudgmentDto.Strengths;
            valueJudgment.BehaviorToCorrect = valueJudgmentDto.BehaviorToCorrect;
            valueJudgment.RecommendationsForDevelopment = valueJudgmentDto.RecommendationsForDevelopment;
            valueJudgment.IsFinalized = valueJudgmentDto.IsFinalized;

            await _dbContext.SaveChangesAsync();
        }
    }
}