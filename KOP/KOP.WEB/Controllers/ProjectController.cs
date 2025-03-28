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
        private readonly ILogger<ProjectController> _logger;

        public ProjectController(IGradeService gradeService, ILogger<ProjectController> logger)
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

                var selectedUserFullName = HttpContext.Session.GetString("SelectedUserFullName");

                if (selectedUserFullName == null)
                {
                    _logger.LogWarning("SelectedUserFullName is incorrect or not found in session.");
                    return BadRequest("Selected user FullName is not valid.");
                }

                var gradeDto = await _gradeService.GetGradeDto(gradeId, new List<GradeEntities> { GradeEntities.Projects });
                var editAccess = (User.IsInRole("Cup") && !gradeDto.IsProjectsFinalized) || User.IsInRole("Urp");
                var viewAccess = gradeDto.IsProjectsFinalized || editAccess;

                var viewModel = new ProjectsViewModel
                {
                    GradeId = gradeId,
                    SelectedUserId = selectedUserId.Value,
                    SelectedUserFullName = selectedUserFullName,
                    Qn2 = gradeDto.Qn2,
                    Projects = gradeDto.ProjectDtoList,
                    EditAccess = editAccess,
                    ViewAccess = viewAccess,
                    IsFinalized = gradeDto.IsProjectsFinalized
                };

                return View("_ProjectsPartial", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ProjectController.GetPopup] : ");
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
            if (viewModel.GradeId <= 0)
            {
                _logger.LogWarning("Invalid gradeId: {gradeId}", viewModel.GradeId);
                return BadRequest("Invalid grade ID.");
            }
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
                _logger.LogError(ex, "[ProjectController.EditAll] : ");
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
                _logger.LogWarning("Invalid projectId: {id}", id);
                return BadRequest("Invalid project ID.");
            }
            try
            {
                await _gradeService.DeleteProject(id);

                return Ok("Удаление прошло успешно");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ProjectController.Delete] : ");
                return BadRequest(new
                {
                    error = "Произошла ошибка при удалении.",
                    details = ex.Message
                });
            }
        }
    }
}