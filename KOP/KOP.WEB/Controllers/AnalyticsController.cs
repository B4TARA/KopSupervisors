using KOP.BLL.Interfaces;
using KOP.Common.Dtos.AnalyticsDtos;
using KOP.DAL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KOP.WEB.Controllers
{
    public class AnalyticsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAssessmentService _assessmentService;

        public AnalyticsController(IUnitOfWork unitOfWork, IAssessmentService assessmentService)
        {
            _unitOfWork = unitOfWork;
            _assessmentService = assessmentService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAssessmentAnalytics(int userId)
        {
            var user = await _unitOfWork.Users.GetAsync(x => x.Id == userId, includeProperties: "Grades.Assessments.AssessmentType");

            if (user == null)
            {
                return NotFound(new { Message = $"Не удалось найти пользователя с ID {userId}" });
            }

            var lastGrade = user.Grades.OrderByDescending(x => x.Number).FirstOrDefault();

            if (lastGrade == null)
            {
                return NotFound(new { Message = $"Не удалось найти последнюю оценку у пользователя с ID {userId}" });
            }

            var assessmentTypesAnalyticsList = new List<AssessmentTypeAnalyticsDto>();

            foreach (var assessment in lastGrade.Assessments)
            {
                var assessmentSummaryDto = await _assessmentService.GetAssessmentSummary(assessment.Id);

                var assessmentTypeAnalyticsDto = new AssessmentTypeAnalyticsDto
                {
                    TypeName = assessment.AssessmentType.Name,
                    GeneralAvgValue = assessmentSummaryDto.GetGeneralAverageValue(),
                    SelfAvgValue = assessmentSummaryDto.AverageSelfValue,
                    SupervisorAvgValue = assessmentSummaryDto.AverageSupervisorValue,
                    ColleaguesAvgValue = assessmentSummaryDto.GetGeneralColleaguesValue(),
                    SelfDataArray = new List<double>(),
                    SupervisorDataArray = new List<double>(),
                    ColleaguesDataArray = new List<double>(),
                    LabelsArray = new List<string>()
                };

                // Сортируем строки с элементами
                var rowsWithElements = assessmentSummaryDto.RowsWithElements
                    .OrderBy(x => x.Key)
                    .ToList();

                // Сортируем и добавляем значения самооценки
                assessmentTypeAnalyticsDto.SelfDataArray.AddRange(
                    assessmentSummaryDto.SelfAssessmentResultValues
                    .OrderBy(x => x.AssessmentMatrixRow)
                    .Select(value => value.Value)
                    .ToList());

                // Сортируем и добавляем значения оценки от руководителей
                assessmentTypeAnalyticsDto.SupervisorDataArray.AddRange(
                    assessmentSummaryDto.SupervisorAssessmentResultValues
                    .OrderBy(x => x.AssessmentMatrixRow)
                    .Select(value => value.Value)
                    .ToList());

                // Сортируем и добавляем значения оценки от коллег
                assessmentTypeAnalyticsDto.ColleaguesDataArray.AddRange(
                    assessmentSummaryDto.ColleaguesAssessmentResultValues
                    .OrderBy(x => x.AssessmentMatrixRow)
                    .Select(value => value.Value)
                    .ToList());

                // Добавляем названия компетенций в LabelsArray
                assessmentTypeAnalyticsDto.LabelsArray = rowsWithElements
                    .Skip(1)
                    .Select(rowElementsGroup => rowElementsGroup.OrderBy(x => x.Column).FirstOrDefault()?.Value ?? "-")
                    .ToList();

                assessmentTypesAnalyticsList.Add(assessmentTypeAnalyticsDto);
            }

            return Json(assessmentTypesAnalyticsList);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetCompetenciesAnalytics(int userId)
        {
            var user = await _unitOfWork.Users.GetAsync(x => x.Id == userId, includeProperties: "Grades.Assessments.AssessmentType");
            if (user is null)
            {
                return NotFound(new { Message = $"Не удалось найти пользователя с ID {userId}" });
            }

            var lastGrade = user.Grades.OrderByDescending(x => x.Number).FirstOrDefault();
            if (lastGrade is null)
            {
                return NotFound(new { Message = $"Не удалось найти последнюю оценку у пользователя с ID {userId}" });
            }

            var competenciesAnalytics = new CompetenciesAnalyticsDto();
            var allCompetencies = new List<CompetenceDto>();

            foreach (var assessment in lastGrade.Assessments)
            {
                var assessmentSummaryDto = await _assessmentService.GetAssessmentSummary(assessment.Id);

                // Сортируем строки и средние значения один раз
                var rowsWithElements = assessmentSummaryDto.RowsWithElements.OrderBy(x => x.Key).ToList();
                var averageValuesByRow = assessmentSummaryDto.AverageValuesByRow.OrderBy(x => x.AssessmentMatrixRow).ToList();

                foreach (var value in averageValuesByRow)
                {
                    var row = rowsWithElements.FirstOrDefault(x => x.Key == (value.AssessmentMatrixRow + 1)); // т.к 1-ый элемент - это заголовок
                    if (row == null)
                    {
                        continue;
                    }

                    var competenceDto = new CompetenceDto
                    {
                        avgValue = value.Value,
                        Name = row.OrderBy(x => x.Column).FirstOrDefault()?.Value ?? "-",
                        CompetenceDescription = row.ElementAtOrDefault(value.AssessmentMatrixRow)?.Value ?? "-"
                    };

                    allCompetencies.Add(competenceDto);
                }
            }

            // Получаем топ-5 элементов с самыми большими значениями
            competenciesAnalytics.TopCompetencies = allCompetencies
                .OrderByDescending(c => c.avgValue)
                .Take(5)
                .ToList();

            // Получаем топ-5 элементов с самыми низкими значениями
            competenciesAnalytics.AntiTopCompetencies = allCompetencies
                .OrderBy(c => c.avgValue)
                .Take(5)
                .ToList();

            // Возврат JSON с типами assessment
            return Json(competenciesAnalytics);
        }
    }
}