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
                var gradeRes = await _gradeService.GetGrade(gradeId, new List<GradeEntities> { GradeEntities.StrategicTasks });

                if (!gradeRes.HasData)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = gradeRes.StatusCode,
                        Message = gradeRes.Description,
                    });
                }

                var userId = Convert.ToInt32(User.FindFirstValue("Id"));
                var conclusionEditAccess = User.IsInRole("Urp");
                var editAccess = (gradeRes.Data.UserId == userId && User.IsInRole("Employee") && !gradeRes.Data.IsStrategicTasksFinalized) || User.IsInRole("Urp");
                var viewAccess = gradeRes.Data.IsStrategicTasksFinalized || editAccess;

                var viewModel = new StrategicTasksViewModel
                {
                    GradeId = gradeId,
                    Conclusion = gradeRes.Data.StrategicTasksConclusion,
                    StrategicTasks = gradeRes.Data.StrategicTasks,
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
                var getGradeRes = await _gradeService.GetGrade(viewModel.GradeId, new List<GradeEntities> { GradeEntities.StrategicTasks });

                if (!getGradeRes.HasData)
                {
                    return BadRequest(new
                    {
                        error = "Произошла ошибка при сохранении.",
                        details = getGradeRes.Description,
                    });
                }

                getGradeRes.Data.StrategicTasks = viewModel.StrategicTasks;
                getGradeRes.Data.StrategicTasksConclusion = viewModel.Conclusion;
                getGradeRes.Data.IsStrategicTasksFinalized = viewModel.IsFinalized;

                var editGradeRes = await _gradeService.EditGrade(getGradeRes.Data);

                if (!editGradeRes.IsSuccess)
                {
                    return BadRequest(new
                    {
                        error = "Произошла ошибка при сохранении.",
                        details = getGradeRes.Description,
                    });
                }

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
                var deleteStrategicTaskRes = await _gradeService.DeleteStrategicTask(id);

                if (!deleteStrategicTaskRes.IsSuccess)
                {
                    return BadRequest(new
                    {
                        error = "Произошла ошибка при удалении.",
                        details = deleteStrategicTaskRes.Description,
                    });
                }

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