using KOP.BLL.Interfaces;
using KOP.Common.Enums;
using KOP.WEB.Models.ViewModels.Shared;
using KOP.WEB.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StatusCodes = KOP.Common.Enums.StatusCodes;

namespace KOP.WEB.Controllers
{
    public class ProjectController : Controller
    {
        private readonly IGradeService _gradeService;

        public ProjectController(IGradeService gradeService)
        {
            _gradeService = gradeService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetPopup(int gradeId)
        {
            try
            {
                var gradeRes = await _gradeService.GetGrade(gradeId, new List<GradeEntities> { GradeEntities.Projects });

                if (!gradeRes.HasData)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = gradeRes.StatusCode,
                        Message = gradeRes.Description,
                    });
                }

                var editAccess = (User.IsInRole("Cup") && !gradeRes.Data.IsProjectsFinalized) || User.IsInRole("Urp");
                var viewAccess = gradeRes.Data.IsProjectsFinalized || editAccess;

                var viewModel = new ProjectsViewModel
                {
                    GradeId = gradeId,
                    Projects = gradeRes.Data.Projects,
                    EditAccess = editAccess,
                    ViewAccess = viewAccess,
                };

                return View("_Projects", viewModel);
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
        public async Task<IActionResult> EditAll(ProjectsViewModel viewModel)
        {
            try
            {
                var getGradeRes = await _gradeService.GetGrade(viewModel.GradeId, new List<GradeEntities> { GradeEntities.Projects });

                if (!getGradeRes.HasData)
                {
                    return BadRequest(new
                    {
                        error = "Произошла ошибка при сохранении.",
                        details = getGradeRes.Description,
                    });
                }

                getGradeRes.Data.Projects = viewModel.Projects;
                getGradeRes.Data.IsProjectsFinalized = viewModel.IsFinalized;

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
                var deleteProjectRes = await _gradeService.DeleteProject(id);

                if (!deleteProjectRes.IsSuccess)
                {
                    return BadRequest(new
                    {
                        error = "Произошла ошибка при удалении.",
                        details = deleteProjectRes.Description,
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