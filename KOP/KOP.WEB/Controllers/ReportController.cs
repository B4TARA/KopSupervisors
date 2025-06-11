using KOP.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KOP.WEB.Controllers
{
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;
        private readonly ILogger<ReportController> _logger;

        public ReportController(IReportService reportService, ILogger<ReportController> logger)
        {
            _reportService = reportService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Supervisor, Urp, Curator, Uop, Umst, Cup")]
        public async Task<IActionResult> GetGradesReport(int gradeId)
        {
            if (gradeId <= 0)
            {
                _logger.LogWarning("Invalid gradeId: {id}", gradeId);
                return BadRequest("Invalid grade ID.");
            }
            try
            {
                var document = await _reportService.GenerateGradesReport(gradeId);
                return File(document, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ReportController.GetGradesReport] : ");
                return BadRequest(new
                {
                    error = "Произошла ошибка при генерации документа.",
                    details = ex.Message
                });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Supervisor, Urp, Curator, Uop, Umst, Cup")]
        public async Task<IActionResult> GetUpcomingGradesReport()
        {
            try
            {
                var currentUserId = Convert.ToInt32(User.FindFirstValue("Id"));

                if (currentUserId <= 0)
                {
                    _logger.LogWarning("CurrentUserId is incorrect or not found in claims.");
                    return BadRequest("Current user ID is not valid.");
                }

                var document = await _reportService.GenerateUpcomingGradesReport(currentUserId);
                return File(document, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ReportController.GetUpcomingGradesReport] : ");
                return BadRequest(new
                {
                    error = "Произошла ошибка при генерации документа.",
                    details = ex.Message
                });
            }
        }
    }
}