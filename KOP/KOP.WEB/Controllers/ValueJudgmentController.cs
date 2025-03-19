using KOP.BLL.Interfaces;
using KOP.Common.Enums;
using KOP.WEB.Models.ViewModels.Shared;
using KOP.WEB.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StatusCodes = KOP.Common.Enums.StatusCodes;

namespace KOP.WEB.Controllers
{
    public class ValueJudgmentController : Controller
    {
        private readonly IGradeService _gradeService;
        private readonly ILogger<ValueJudgmentController> _logger;

        public ValueJudgmentController(IGradeService gradeService, ILogger<ValueJudgmentController> logger)
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
                var gradeDto = await _gradeService.GetGradeDto(gradeId, new List<GradeEntities> { GradeEntities.ValueJudgment });

                if (gradeDto.ValueJudgmentDto == null)
                {
                    throw new Exception($"ValueJudgment is null for Grade with ID {gradeId}.");
                }

                var editAccess = (User.IsInRole("Supervisor") && !gradeDto.IsValueJudgmentFinalized) || User.IsInRole("Urp");
                var viewAccess = gradeDto.IsValueJudgmentFinalized || editAccess;

                var viewModel = new ValueJudgmentViewModel
                {
                    GradeId = gradeId,
                    // CHTCK THIS !!!
                    SelectedUserId = HttpContext.Session.GetInt32("SelectedUserId") ?? 0,
                    ValueJudgmentDto = gradeDto.ValueJudgmentDto,
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
                var gradeDto = await _gradeService.GetGradeDto(viewModel.GradeId, new List<GradeEntities> { GradeEntities.ValueJudgment });

                gradeDto.ValueJudgmentDto = viewModel.ValueJudgmentDto;
                gradeDto.IsValueJudgmentFinalized = viewModel.IsFinalized;

                await _gradeService.EditGrade(gradeDto);

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