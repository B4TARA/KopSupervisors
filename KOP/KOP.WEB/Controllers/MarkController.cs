using KOP.BLL.Interfaces;
using KOP.Common.Enums;
using KOP.WEB.Models.ViewModels.Shared;
using KOP.WEB.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StatusCodes = KOP.Common.Enums.StatusCodes;

namespace KOP.WEB.Controllers
{
    public class MarkController : Controller
    {
        private readonly IGradeService _gradeService;
        private readonly ILogger<MarkController> _logger;
        public MarkController(IGradeService gradeService, ILogger<MarkController> logger)
        {
            _gradeService = gradeService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Supervisor, Urp, Curator, Uop, Employee")]
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

                var gradeDto = await _gradeService.GetGradeDto(gradeId, new List<GradeEntities> { GradeEntities.Marks });
                var editAccess = User.IsInRole("Urp");
                var viewAccess = gradeDto.IsMarksFinalized || editAccess;

                var viewModel = new MarksViewModel
                {
                    GradeId = gradeId,
                    SelectedUserId = selectedUserId.Value,
                    MarkTypes = gradeDto.MarkTypeDtoList,
                    EditAccess = editAccess,
                    ViewAccess = viewAccess,
                };

                return View("_MarksPartial", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[MarkController.GetPopup] : ");
                return View("Error", new ErrorViewModel
                {
                    StatusCode = StatusCodes.InternalServerError,
                    Message = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Urp")]
        public async Task<IActionResult> EditAll(MarksViewModel viewModel)
        {
            if (viewModel.GradeId <= 0)
            {
                _logger.LogWarning("Invalid gradeId: {gradeId}", viewModel.GradeId);
                return BadRequest("Invalid grade ID.");
            }

            try
            {
                var gradeDto = await _gradeService.GetGradeDto(viewModel.GradeId, new List<GradeEntities> { GradeEntities.Marks });

                gradeDto.MarkTypeDtoList = viewModel.MarkTypes;
                gradeDto.IsMarksFinalized = viewModel.IsFinalized;

                await _gradeService.EditGrade(gradeDto);

                return Ok(viewModel.IsFinalized ? "Окончательное сохранение прошло успешно" : "Сохранение черновика прошло успешно");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[MarkController.EditAll] : ");
                return BadRequest(new
                {
                    error = "Произошла ошибка при сохранении.",
                    details = ex.Message
                });
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Urp")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid markId: {id}", id);
                return BadRequest("Invalid mark ID.");
            }
            try
            {
                await _gradeService.DeleteMark(id);

                return Ok("Удаление прошло успешно");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[MarkController.Delete] : ");
                return BadRequest(new
                {
                    error = "Произошла ошибка при удалении.",
                    details = ex.Message
                });
            }
        }
    }
}