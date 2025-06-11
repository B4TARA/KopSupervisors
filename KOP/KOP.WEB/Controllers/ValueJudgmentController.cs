using KOP.BLL.Interfaces;
using KOP.Common.Dtos.GradeDtos;
using KOP.WEB.Models.ViewModels;
using KOP.WEB.Models.ViewModels.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StatusCodes = KOP.Common.Enums.StatusCodes;

namespace KOP.WEB.Controllers
{
    public class ValueJudgmentController : Controller
    {
        private readonly IValueJudgmentService _valueJudgmentService;
        private readonly ILogger<ValueJudgmentController> _logger;

        public ValueJudgmentController(IValueJudgmentService valueJudgmentService, ILogger<ValueJudgmentController> logger)
        {
            _valueJudgmentService = valueJudgmentService;
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

                var valueJudgment = await _valueJudgmentService.GetValueJudgmentForGrade(gradeId);
                var editAccess = (User.IsInRole("Supervisor") && !valueJudgment.IsFinalized) || User.IsInRole("Urp");
                var viewAccess = valueJudgment.IsFinalized || editAccess;

                var viewModel = new ValueJudgmentViewModel
                {
                    Id = valueJudgment.Id,
                    GradeId = valueJudgment.GradeId,
                    SelectedUserId = selectedUserId.Value,
                    Strengths = valueJudgment.Strengths,
                    BehaviorToCorrect = valueJudgment.BehaviorToCorrect,
                    RecommendationsForDevelopment = valueJudgment.RecommendationsForDevelopment,
                    EditAccess = editAccess,
                    ViewAccess = viewAccess,
                };

                return View("_ValueJudgmentPartial", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ValueJudgmentController.GetPopup] : ");

                return View("Error", new ErrorViewModel
                {
                    StatusCode = StatusCodes.InternalServerError,
                    Message = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(ValueJudgmentViewModel viewModel)
        {
            if (viewModel.GradeId <= 0)
            {
                _logger.LogWarning("Invalid gradeId: {gradeId}", viewModel.GradeId);

                return BadRequest("Invalid grade ID.");
            }

            try
            {
                var valueJudgmentDto = new ValueJudgmentDto
                {
                    Id = viewModel.Id,
                    GradeId = viewModel.GradeId,
                    Strengths = viewModel.Strengths,
                    BehaviorToCorrect = viewModel.BehaviorToCorrect,
                    RecommendationsForDevelopment = viewModel.RecommendationsForDevelopment,
                    IsFinalized = viewModel.IsFinalized
                };

                await _valueJudgmentService.EditValueJudgment(valueJudgmentDto);

                return Ok(viewModel.IsFinalized ? "Окончательное сохранение прошло успешно" : "Сохранение черновика прошло успешно");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ValueJudgmentController.Edit] : ");

                return BadRequest(new
                {
                    error = "Произошла ошибка при сохранении.",
                    details = ex.Message
                });
            }
        }
    }
}