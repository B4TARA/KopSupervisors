using KOP.BLL.Interfaces;
using KOP.Common.Enums;
using KOP.WEB.Models.ViewModels.Shared;
using KOP.WEB.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StatusCodes = KOP.Common.Enums.StatusCodes;
using System.Security.Claims;

namespace KOP.WEB.Controllers
{
    public class StrategicTaskController : Controller
    {
        private readonly IGradeService _gradeService;
        private readonly ILogger<StrategicTaskController> _logger;

        public StrategicTaskController(IGradeService gradeService, ILogger<StrategicTaskController> logger)
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

                var currentUserId = Convert.ToInt32(User.FindFirstValue("Id"));

                if (currentUserId <= 0)
                {
                    _logger.LogWarning("CurrentUserId is incorrect or not found in claims.");
                    return BadRequest("Current user ID is not valid.");
                }

                var gradeDto = await _gradeService.GetGradeDto(gradeId, new List<GradeEntities> { GradeEntities.StrategicTasks });
                var conclusionEditAccess = User.IsInRole("Urp");
                var editAccess = (gradeDto.UserId == currentUserId && User.IsInRole("Employee") && !gradeDto.IsStrategicTasksFinalized) || User.IsInRole("Urp");
                var viewAccess = gradeDto.IsStrategicTasksFinalized || editAccess;

                var viewModel = new StrategicTasksViewModel
                {
                    GradeId = gradeId,
                    SelectedUserId = selectedUserId.Value,
                    Conclusion = gradeDto.StrategicTasksConclusion,
                    StrategicTaskDtoList = gradeDto.StrategicTaskDtoList,
                    EditAccess = editAccess,
                    ConclusionEditAccess = conclusionEditAccess,
                    ViewAccess = viewAccess,
                };

                return View("_StrategicTasksPartial", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[StrategicTaskController.GetPopup] : ");
                return View("Error", new ErrorViewModel
                {
                    StatusCode = StatusCodes.InternalServerError,
                    Message = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Urp, Employee")]
        public async Task<IActionResult> EditAll(StrategicTasksViewModel viewModel)
        {
            if (viewModel.GradeId <= 0)
            {
                _logger.LogWarning("Invalid gradeId: {gradeId}", viewModel.GradeId);
                return BadRequest("Invalid grade ID.");
            }

            try
            {
                var gradeDto = await _gradeService.GetGradeDto(viewModel.GradeId, new List<GradeEntities> { GradeEntities.StrategicTasks });

                gradeDto.StrategicTaskDtoList = viewModel.StrategicTaskDtoList;
                gradeDto.StrategicTasksConclusion = viewModel.Conclusion;
                gradeDto.IsStrategicTasksFinalized = viewModel.IsFinalized;

                await _gradeService.EditGrade(gradeDto);

                return Ok(viewModel.IsFinalized ? "Окончательное сохранение прошло успешно" : "Сохранение черновика прошло успешно");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[StrategicTaskController.EditAll] : ");
                return BadRequest(new
                {
                    error = "Произошла ошибка при сохранении.",
                    details = ex.Message
                });
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Urp, Employee")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid strategicTaskId: {id}", id);
                return BadRequest("Invalid strategicTask ID.");
            }

            try
            {
                await _gradeService.DeleteStrategicTask(id);

                return Ok("Удаление прошло успешно");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[StrategicTaskController.Delete] : ");
                return BadRequest(new
                {
                    error = "Произошла ошибка при удалении.",
                    details = ex.Message
                });
            }
        }
    }
}