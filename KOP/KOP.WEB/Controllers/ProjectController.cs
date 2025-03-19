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
                var gradeDto = await _gradeService.GetGradeDto(gradeId, new List<GradeEntities> { GradeEntities.Projects });
                var editAccess = (User.IsInRole("Cup") && !gradeDto.IsProjectsFinalized) || User.IsInRole("Urp");
                var viewAccess = gradeDto.IsProjectsFinalized || editAccess;

                var viewModel = new ProjectsViewModel
                {
                    GradeId = gradeId,
                    Qn2 = gradeDto.Qn2,
                    SelectedUserFullName = HttpContext.Session.GetString("SelectedUserFullName") ?? "-",
                    Projects = gradeDto.ProjectDtoList,
                    EditAccess = editAccess,
                    ViewAccess = viewAccess,
                };

                return View("_ProjectsPartial", viewModel);
            }
            catch
            {
                // LOG!!!
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
                var gradeDto = await _gradeService.GetGradeDto(viewModel.GradeId, new List<GradeEntities> { GradeEntities.Projects });

                gradeDto.ProjectDtoList = viewModel.Projects;
                gradeDto.IsProjectsFinalized = viewModel.IsFinalized;
                gradeDto.Qn2 = viewModel.Qn2;

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
                await _gradeService.DeleteProject(id);

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