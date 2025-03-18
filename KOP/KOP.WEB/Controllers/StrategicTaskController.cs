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

        public StrategicTaskController(IGradeService gradeService)
        {
            _gradeService = gradeService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetPopup(int gradeId)
        {
            try
            {
                var gradeDto = await _gradeService.GetGradeDto(gradeId, new List<GradeEntities> { GradeEntities.StrategicTasks });

                var userId = Convert.ToInt32(User.FindFirstValue("Id"));
                var conclusionEditAccess = User.IsInRole("Urp");
                var editAccess = (gradeDto.UserId == userId && User.IsInRole("Employee") && !gradeDto.IsStrategicTasksFinalized) || User.IsInRole("Urp");
                var viewAccess = gradeDto.IsStrategicTasksFinalized || editAccess;

                var viewModel = new StrategicTasksViewModel
                {
                    GradeId = gradeId,
                    Conclusion = gradeDto.StrategicTasksConclusion,
                    StrategicTasks = gradeDto.StrategicTasks,
                    EditAccess = editAccess,
                    ConclusionEditAccess = conclusionEditAccess,
                    ViewAccess = viewAccess,
                };

                return View("_StrategicTasks", viewModel);
            }
            catch
            {
                return View("Error", new ErrorViewModel
                {
                    StatusCode = StatusCodes.InternalServerError,
                    Message = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditAll(StrategicTasksViewModel viewModel)
        {
            try
            {
                var gradeDto = await _gradeService.GetGradeDto(viewModel.GradeId, new List<GradeEntities> { GradeEntities.StrategicTasks });

                gradeDto.StrategicTasks = viewModel.StrategicTasks;
                gradeDto.StrategicTasksConclusion = viewModel.Conclusion;
                gradeDto.IsStrategicTasksFinalized = viewModel.IsFinalized;

                await _gradeService.EditGrade(gradeDto);

                return Ok(viewModel.IsFinalized ? "Окончательное сохранение прошло успешно" : "Сохранение черновика прошло успешно");
            }
            catch (Exception ex)
            {
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
            try
            {
                await _gradeService.DeleteStrategicTask(id);

                return Ok("Удаление прошло успешно");
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    error = "Произошла ошибка при удалении.",
                    details = ex.Message
                });
            }
        }
    }
}