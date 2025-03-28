using KOP.BLL.Interfaces;
using KOP.Common.Enums;
using KOP.WEB.Models.ViewModels.Shared;
using KOP.WEB.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StatusCodes = KOP.Common.Enums.StatusCodes;

namespace KOP.WEB.Controllers
{
    public class QualificationController : Controller
    {
        private readonly IGradeService _gradeService;
        private readonly ILogger<QualificationController> _logger;

        public QualificationController(IGradeService gradeService, ILogger<QualificationController> logger)
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

                var gradeDto = await _gradeService.GetGradeDto(gradeId, new List<GradeEntities> { GradeEntities.Qualification });

                if (gradeDto.QualificationDto == null)
                {
                    throw new Exception($"Qualification is null for Grade with ID {gradeId}.");
                }

                var conclusionEditAccess = User.IsInRole("Urp");
                var editAccess = User.IsInRole("Urp");
                var viewAccess = gradeDto.IsQualificationFinalized || editAccess;

                var viewModel = new QualificationViewModel
                {
                    GradeId = gradeId,
                    SelectedUserFullName = selectedUserFullName,
                    SelectedUserId = selectedUserId.Value,
                    Conclusion = gradeDto.QualificationConclusion,
                    Qualification = gradeDto.QualificationDto,
                    EditAccess = editAccess,
                    ViewAccess = viewAccess,
                    ConclusionEditAccess = conclusionEditAccess,
                };

                return View("_QualificationPartial", viewModel);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "[QualificationController.GetPopup] : ");
                return View("Error", new ErrorViewModel
                {
                    StatusCode = StatusCodes.InternalServerError,
                    Message = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(QualificationViewModel viewModel)
        {
            if (viewModel.GradeId <= 0)
            {
                _logger.LogWarning("Invalid gradeId: {gradeId}", viewModel.GradeId);
                return BadRequest("Invalid grade ID.");
            }
            try
            {
                var gradeDto = await _gradeService.GetGradeDto(viewModel.GradeId, new List<GradeEntities> { GradeEntities.Qualification });

                gradeDto.QualificationDto = viewModel.Qualification;
                gradeDto.QualificationConclusion = viewModel.Conclusion;
                gradeDto.IsQualificationFinalized = viewModel.IsFinalized;

                await _gradeService.EditGrade(gradeDto);

                return Ok(viewModel.IsFinalized ? "Окончательное сохранение прошло успешно" : "Сохранение черновика прошло успешно");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[QualificationController.Edit] : ");
                return BadRequest(new
                {
                    error = "Произошла ошибка при сохранении.",
                    details = ex.Message
                });
            }
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeletePreviousJob(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid previousJobId: {id}", id);
                return BadRequest("Invalid previousJob ID.");
            }
            try
            {
                await _gradeService.DeletePreviousJob(id);

                return Ok("Удаление прошло успешно");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[QualificationController.DeletePreviousJob] : ");
                return BadRequest(new
                {
                    error = "Произошла ошибка при удалении.",
                    details = ex.Message
                });
            }
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteHigherEducation(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid higherEducationId: {id}", id);
                return BadRequest("Invalid higherEducation ID.");
            }
            try
            {
                await _gradeService.DeleteHigherEducation(id);

                return Ok("Удаление прошло успешно");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[QualificationController.DeleteHigherEducation] : ");
                return BadRequest(new
                {
                    error = "Произошла ошибка при удалении.",
                    details = ex.Message
                });
            }
        }
    }
}