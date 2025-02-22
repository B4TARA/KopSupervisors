﻿using System.Security.Claims;
using System.Security.Cryptography.Xml;
using KOP.BLL.Interfaces;
using KOP.Common.Dtos.AnalyticsDtos;
using KOP.DAL.Interfaces;
using KOP.WEB.Models.ViewModels;
using KOP.WEB.Models.ViewModels.Analytics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace KOP.WEB.Controllers
{
    public class AnalyticsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAssessmentService _assessmentService;
        private readonly IReportService _reportService;

        public AnalyticsController(IUnitOfWork unitOfWork, IAssessmentService assessmentService, IReportService reportService)
        {
            _unitOfWork = unitOfWork;
            _assessmentService = assessmentService;
            _reportService = reportService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAnalyticsLayout()
        {
            var currentUserId = Convert.ToInt32(User.FindFirstValue("Id"));
            var getSubordinateUsersRes = await _reportService.GetSubordinateUsersWithGrade(currentUserId);
            if (!getSubordinateUsersRes.HasData)
            {
                return View("Error", new ErrorViewModel
                {
                    StatusCode = getSubordinateUsersRes.StatusCode,
                    Message = getSubordinateUsersRes.Description,
                });
            }

            var subordinateUsers = getSubordinateUsersRes.Data;
            var viewModel = new AnalyticsLayoutViewModel
            {
                SubordinateUsers = subordinateUsers,
            };

            return View("AnalyticsLayout", viewModel);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAssessmentAnalytics(int userId)
        {
            var user = await _unitOfWork.Users.GetAsync(x => x.Id == userId, includeProperties: "Grades.Assessments.AssessmentType");
            if (user is null)
            {
                return NotFound(new { Message = $"Не удалось найти пользователя с ID {userId}" });
            }

            var lastGrade = user.Grades.OrderByDescending(x => x.DateOfCreation).FirstOrDefault();
            if (lastGrade is null)
            {
                return NotFound(new { Message = $"Не удалось найти последнюю оценку у пользователя с ID {userId}" });
            }

            var assessmentTypesAnalyticsList = new List<AssessmentTypeAnalyticsDto>();
            foreach (var assessment in lastGrade.Assessments)
            {
                var getAssessmentSummaryRes = await _assessmentService.GetAssessmentSummary(assessment.Id);
                if (!getAssessmentSummaryRes.HasData)
                {
                    return StatusCode((int)getAssessmentSummaryRes.StatusCode, new { Message = getAssessmentSummaryRes.Description });
                }

                var assessmentSummary = getAssessmentSummaryRes.Data;
                var assessmentTypeAnalyticsDto = new AssessmentTypeAnalyticsDto
                {
                    TypeName = assessment.AssessmentType.Name,
                    GeneralAvgValue = assessmentSummary.GetGeneralAverageValue(),
                    SelfAvgValue = assessmentSummary.AverageSelfValue,
                    SupervisorAvgValue = assessmentSummary.AverageSupervisorValue,
                    ColleaguesAvgValue = assessmentSummary.GetGeneralColleaguesValue(),

                };
                var rowsWithElements = assessmentSummary.RowsWithElements.OrderBy(x => x.Key);
                var selfAssessmentValues = assessmentSummary.SelfAssessmentResultValues.OrderBy(x => x.AssessmentMatrixRow);
                var supervisorAssessmentValues = assessmentSummary.SupervisorAssessmentResultValues.OrderBy(x => x.AssessmentMatrixRow);
                var colleaguesAssessmentValues = assessmentSummary.ColleaguesAssessmentResultValues.OrderBy(x => x.AssessmentMatrixRow);

                foreach (var rowElementsGroup in rowsWithElements.Skip(1))
                {
                    var competenceNameElement = rowElementsGroup.OrderBy(x => x.Column).FirstOrDefault();
                    if (competenceNameElement != null)
                    {
                        assessmentTypeAnalyticsDto.LabelsArray.Add(competenceNameElement.Value);
                    }
                    else
                    {
                        assessmentTypeAnalyticsDto.LabelsArray.Add("-");
                    }
                }

                foreach (var selfValue in selfAssessmentValues)
                {
                    assessmentTypeAnalyticsDto.SelfDataArray.Add(selfValue.Value);
                }

                foreach (var supervisorValue in supervisorAssessmentValues)
                {
                    assessmentTypeAnalyticsDto.SupervisorDataArray.Add(supervisorValue.Value);
                }

                foreach (var colleagueValue in colleaguesAssessmentValues)
                {
                    assessmentTypeAnalyticsDto.ColleaguesDataArray.Add(colleagueValue.Value);
                }

                assessmentTypesAnalyticsList.Add(assessmentTypeAnalyticsDto);
            }

            // Сериализация списка в JSON
            var json = JsonConvert.SerializeObject(assessmentTypesAnalyticsList);

            // Возврат JSON с типами assessment
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

            var lastGrade = user.Grades.OrderByDescending(x => x.DateOfCreation).FirstOrDefault();
            if (lastGrade is null)
            {
                return NotFound(new { Message = $"Не удалось найти последнюю оценку у пользователя с ID {userId}" });
            }

            var competenciesAnalytics = new CompetenciesAnalyticsDto();
            var allCompetencies = new List<CompetenceDto>();

            foreach (var assessment in lastGrade.Assessments)
            {
                var getAssessmentSummaryRes = await _assessmentService.GetAssessmentSummary(assessment.Id);
                if (!getAssessmentSummaryRes.HasData)
                {
                    return StatusCode((int)getAssessmentSummaryRes.StatusCode, new { Message = getAssessmentSummaryRes.Description });
                }

                var assessmentSummary = getAssessmentSummaryRes.Data;

                // Сортируем строки и средние значения один раз
                var rowsWithElements = assessmentSummary.RowsWithElements.OrderBy(x => x.Key).ToList();
                var averageValuesByRow = assessmentSummary.AverageValuesByRow.OrderBy(x => x.AssessmentMatrixRow).ToList();

                foreach (var value in averageValuesByRow)
                {
                    var row = rowsWithElements.FirstOrDefault(x => x.Key == (value.AssessmentMatrixRow + 1)); // т.к 1-ый элемент - это заголовок
                    if (row == null)
                    {
                        continue; // Пропускаем, если строка не найдена
                    }

                    var competenceDto = new CompetenceDto
                    {
                        avgValue = value.Value,
                        Name = row.OrderBy(x => x.Column).FirstOrDefault()?.Value ?? "-",
                        CompetenceDescription = row.ElementAtOrDefault(value.AssessmentMatrixRow)?.Value ?? "-"
                    };

                    allCompetencies.Add(competenceDto); // Добавляем созданный объект в список
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