using KOP.BLL.Interfaces;
using KOP.Common.Dtos.GradeDtos;
using KOP.WEB.Models.ViewModels;
using KOP.WEB.Models.ViewModels.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StatusCodes = KOP.Common.Enums.StatusCodes;

namespace KOP.WEB.Controllers
{
    public class QualificationController : Controller
    {
        private readonly IQualificationService _qualificationService;
        private readonly ILogger<QualificationController> _logger;

        public QualificationController(IQualificationService qualificationService, ILogger<QualificationController> logger)
        {
            _qualificationService = qualificationService;
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

                var selectedUserFullName = HttpContext.Session.GetString("SelectedUserFullName");

                if (selectedUserFullName == null)
                {
                    _logger.LogWarning("SelectedUserFullName is incorrect or not found in session.");
                    return BadRequest("Selected user FullName is not valid.");
                }

                var qualification = await _qualificationService.GetQualificationForGrade(gradeId);
                var conclusionEditAccess = User.IsInRole("Urp");
                var editAccess = User.IsInRole("Urp");
                var viewAccess = qualification.IsFinalized || editAccess;

                var viewModel = new QualificationViewModel
                {
                    Id = qualification.Id,
                    GradeId = qualification.GradeId,
                    SelectedUserId = selectedUserId.Value,
                    SelectedUserFullName = selectedUserFullName,
                    CurrentStatusDate = qualification.CurrentStatusDate,
                    CurrentExperienceYears = qualification.CurrentExperienceYears,
                    CurrentExperienceMonths = qualification.CurrentExperienceMonths,
                    CurrentJobStartDate = qualification.CurrentJobStartDate,
                    CurrentJobPositionName = qualification.CurrentJobPositionName,
                    EmploymentContarctTerminations = qualification.EmploymentContarctTerminations,
                    QualificationResult = qualification.QualificationResult,
                    IsFinalized = qualification.IsFinalized,
                    Conclusion = qualification.Conclusion,
                    PreviousJobs = qualification.PreviousJobs,
                    HigherEducations = qualification.HigherEducations,
                    EditAccess = editAccess,
                    ViewAccess = viewAccess,
                    ConclusionEditAccess = conclusionEditAccess,
                };

                return View("_QualificationPartial", viewModel);
            }
            catch (Exception ex)
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
        [Authorize(Roles = "Urp")]
        public async Task<IActionResult> Edit(QualificationViewModel viewModel)
        {
            if (viewModel.GradeId <= 0)
            {
                _logger.LogWarning("Invalid gradeId: {gradeId}", viewModel.GradeId);
                return BadRequest("Invalid grade ID.");
            }
            try
            {
                var qualificationDto = new QualificationDto
                {
                    Id = viewModel.Id,
                    GradeId = viewModel.GradeId,
                    Conclusion = viewModel.Conclusion,
                    CurrentStatusDate = viewModel.CurrentStatusDate,
                    CurrentExperienceYears = viewModel.CurrentExperienceYears,
                    CurrentExperienceMonths = viewModel.CurrentExperienceMonths,
                    CurrentJobStartDate = viewModel.CurrentJobStartDate,
                    CurrentJobPositionName = viewModel.CurrentJobPositionName,
                    EmploymentContarctTerminations = viewModel.EmploymentContarctTerminations,
                    QualificationResult = viewModel.QualificationResult,
                    IsFinalized = viewModel.IsFinalized,
                    PreviousJobs = viewModel.PreviousJobs,
                    HigherEducations = viewModel.HigherEducations,
                };

                await _qualificationService.EditQualification(qualificationDto);

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
        [Authorize(Roles = "Urp")]
        public async Task<IActionResult> DeletePreviousJob(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid previousJobId: {id}", id);
                return BadRequest("Invalid previousJob ID.");
            }
            try
            {
                await _qualificationService.DeletePreviousJob(id);

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
        [Authorize(Roles = "Urp")]
        public async Task<IActionResult> DeleteHigherEducation(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid higherEducationId: {id}", id);
                return BadRequest("Invalid higherEducation ID.");
            }
            try
            {
                await _qualificationService.DeleteHigherEducation(id);

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