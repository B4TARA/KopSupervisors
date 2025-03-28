using KOP.BLL.Interfaces;
using KOP.Common.Enums;
using KOP.WEB.Models.ViewModels.Shared;
using KOP.WEB.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StatusCodes = KOP.Common.Enums.StatusCodes;

namespace KOP.WEB.Controllers
{
    public class KpiController : Controller
    {
        private readonly IGradeService _gradeService;
        private readonly ILogger<KpiController> _logger;

        public KpiController(IGradeService gradeService, ILogger<KpiController> logger)
        {
            _gradeService = gradeService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetPopup(int gradeId)
        {
            if (gradeId <= 0)
            {
                _logger.LogWarning("Invalid gradeId: {gradeId}", gradeId);
                return BadRequest("Invalid grade ID.");
            }
            try
            {
                var selectedUserId = HttpContext.Session.GetInt32("SelectedUserId");

                if (!selectedUserId.HasValue || selectedUserId <= 0)
                {
                    _logger.LogWarning("SelectedUserId is incorrect or not found in session.");
                    return BadRequest("Selected user ID is not valid.");
                }

                var gradeDto = await _gradeService.GetGradeDto(gradeId, new List<GradeEntities> { GradeEntities.Kpis });
                var conclusionEditAccess = User.IsInRole("Urp");
                var editAccess = (User.IsInRole("Umst") && !gradeDto.IsKpisFinalized) || User.IsInRole("Urp");
                var viewAccess = gradeDto.IsKpisFinalized || editAccess;

                var viewModel = new KpisViewModel
                {
                    GradeId = gradeId,
                    SelectedUserId = selectedUserId.Value,
                    Conclusion = gradeDto.KPIsConclusion,
                    Kpis = gradeDto.KpiDtoList,
                    EditAccess = editAccess,
                    ViewAccess = viewAccess,
                    ConclusionEditAccess = conclusionEditAccess,
                };

                return View("_KpisPartial", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[KpiController.GetPopup] : ");
                return View("Error", new ErrorViewModel
                {
                    StatusCode = StatusCodes.InternalServerError,
                    Message = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditAll(KpisViewModel viewModel)
        {
            if (viewModel.GradeId <= 0)
            {
                _logger.LogWarning("Invalid gradeId: {gradeId}", viewModel.GradeId);
                return BadRequest("Invalid grade ID.");
            }
            try
            {
                var gradeDto = await _gradeService.GetGradeDto(viewModel.GradeId, new List<GradeEntities> { GradeEntities.Kpis });

                gradeDto.KpiDtoList = viewModel.Kpis;
                gradeDto.KPIsConclusion = viewModel.Conclusion;
                gradeDto.IsKpisFinalized = viewModel.IsFinalized;

                await _gradeService.EditGrade(gradeDto);

                return Ok(viewModel.IsFinalized ? "Окончательное сохранение прошло успешно" : "Сохранение черновика прошло успешно");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[KpiController.EditAll] : ");
                return BadRequest(new
                {
                    error = "Произошла ошибка при сохранении.",
                    details = ex.Message
                });
            }
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid kpiId: {id}", id);
                return BadRequest("Invalid kpi ID.");
            }
            try
            {
                await _gradeService.DeleteKpi(id);

                return Ok("Удаление прошло успешно");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[KpiController.Delete] : ");
                return BadRequest(new
                {
                    error = "Произошла ошибка при удалении.",
                    details = ex.Message
                });
            }
        }
    }
}
