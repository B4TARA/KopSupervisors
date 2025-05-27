using KOP.BLL.Interfaces;
using KOP.WEB.Models.RequestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost]
        [Authorize(Roles = "Supervisor, Urp, Curator, Uop, Umst, Cup")]
        public async Task<IActionResult> GenerateGradeWordDocument([FromBody] GenerateGradeWordDocumentRequestModel requestModel)
        {
            if (requestModel.gradeId <= 0)
            {
                _logger.LogWarning("Invalid gradeId: {id}", requestModel.gradeId);
                return BadRequest("Invalid grade ID.");
            }
            try
            {
                var document = await _reportService.GenerateGradeWordDocument(requestModel.gradeId);
                return File(document, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "[ReportController.GenerateGradeWordDocument] : ");
                return BadRequest(new
                {
                    error = "Произошла ошибка при генерации документа.",
                    details = ex.Message
                });
            }
        }
    }
}