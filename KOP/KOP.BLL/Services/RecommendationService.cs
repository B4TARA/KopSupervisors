using KOP.BLL.Interfaces;
using KOP.Common.Dtos.RecommendationDtos;
using KOP.Common.Enums;
using KOP.DAL;
using KOP.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace KOP.BLL.Services
{
    public class RecommendationService : IRecommendationService
    {
        private readonly ApplicationDbContext _context;

        public RecommendationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<RecommendationDto>> GetCourseRecommendationsForGrade(int gradeId)
        {
            var courseRecommendations = await _context.Recommendations
                .AsNoTracking()
                .Where(r => r.GradeId == gradeId && r.Type == RecommendationTypes.Course)
                .Select(r => new RecommendationDto { Id = r.Id, Value = r.Value, GradeId = r.GradeId })
                .ToListAsync();

            return courseRecommendations;
        }

        public async Task<List<RecommendationDto>> GetLiteratureRecommendationsForGrade(int gradeId)
        {
            var literatureRecommendations = await _context.Recommendations
                .AsNoTracking()
                .Where(r => r.GradeId == gradeId && r.Type == RecommendationTypes.Literature)
                .Select(r => new RecommendationDto { Id = r.Id, Value = r.Value, GradeId = r.GradeId })
                .ToListAsync();

            return literatureRecommendations;
        }

        public async Task<List<RecommendationDto>> GetSeminarRecommendationsForGrade(int gradeId)
        {
            var seminarRecommendations = await _context.Recommendations
                .AsNoTracking()
                .Where(r => r.GradeId == gradeId && r.Type == RecommendationTypes.Seminar)
                .Select(r => new RecommendationDto { Id = r.Id, Value = r.Value, GradeId = r.GradeId })
                .ToListAsync();

            return seminarRecommendations;
        }

        public async Task<List<RecommendationDto>> GetCompetenceRecommendationsForGrade(int gradeId)
        {
            var competenceRecommendations = await _context.Recommendations
                .AsNoTracking()
                .Where(r => r.GradeId == gradeId && r.Type == RecommendationTypes.Сompetence)
                .Select(r => new RecommendationDto { Id = r.Id, Value = r.Value, GradeId = r.GradeId })
                .ToListAsync();

            return competenceRecommendations;
        }

        public async Task ProcessRecommendations(List<RecommendationDto> items, RecommendationTypes type)
        {
            foreach (var item in items)
            {
                if (item.IsDeleted && item.Id != 0)
                {
                    await DeleteRecommendation(item.Id);
                }
                else if (item.Id == 0 && !item.IsDeleted)
                {
                    await AddRecommendation(item.Value, type, item.GradeId);
                }
                else if (!item.IsDeleted)
                {
                    await UpdateRecommendation(item.Id, item.Value);
                }
            }
        }

        private async Task AddRecommendation(string value, RecommendationTypes type, int gradeId)
        {
            var newItem = new Recommendation(value, type, gradeId);

            _context.Recommendations.Add(newItem);
            await _context.SaveChangesAsync();
        }

        private async Task UpdateRecommendation(int id, string value)
        {
            var item = await _context.Recommendations.FindAsync(id);
            if (item != null)
            {
                item.Value = value;
                item.DateOfModification = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        private async Task DeleteRecommendation(int id)
        {
            var item = await _context.Recommendations.FindAsync(id);
            if (item != null)
            {
                _context.Recommendations.Remove(item);
                await _context.SaveChangesAsync();
            }
        }
    }
}