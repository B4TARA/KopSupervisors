using KOP.BLL.Interfaces;
using KOP.Common.Enums;
using KOP.WEB.Models.ViewModels;
using KOP.WEB.Models.ViewModels.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StatusCodes = KOP.Common.Enums.StatusCodes;

namespace KOP.WEB.Controllers
{
    public class AssessmentController : Controller
    {
        private readonly IGradeService _gradeService;
        private readonly IUserService _userService;
        private readonly IAssessmentService _assessmentService;
        private readonly ILogger<AssessmentController> _logger;

        public AssessmentController(IGradeService gradeService, IUserService userService, IAssessmentService assessmentService, ILogger<AssessmentController> logger)
        {
            _gradeService = gradeService;
            _userService = userService;
            _assessmentService = assessmentService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetCorporateCompetenciesPopup(int gradeId)
        {
            if (gradeId <= 0)
            {
                _logger.LogWarning("Invalid gradeId: {gradeId}", gradeId);
                return BadRequest("Invalid grade ID.");
            }

            try
            {
                var gradeDto = await _gradeService.GetGradeDto(gradeId, [GradeEntities.Assessments]);

                var corporateAssessmentDto = gradeDto.AssessmentDtoList.FirstOrDefault(a => a.SystemAssessmentType == SystemAssessmentTypes.СorporateСompetencies);

                if (corporateAssessmentDto == null)
                {
                    throw new Exception($"Corporate competencies assessment is null for Grade with ID {gradeId}.");
                }

                var assessmentSummaryDto = await _assessmentService.GetAssessmentSummary(corporateAssessmentDto.Id);

                if (assessmentSummaryDto == null)
                {
                    throw new Exception($"Corporate competencies assessment summary is null for Grade with ID {gradeId}.");
                }

                var viewModel = new CorporateCompetenciesViewModel
                {
                    Conclusion = gradeDto.CorporateCompetenciesConclusion,
                    AssessmentSummaryDto = assessmentSummaryDto,
                };

                return View("_CorporateCompetenciesPartial", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AssessmentController.GetCorporateCompetenciesPopup] : ");

                return View("Error", new ErrorViewModel
                {
                    StatusCode = StatusCodes.InternalServerError,
                    Message = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetManagmentCompetenciesPopup(int gradeId)
        {
            if (gradeId <= 0)
            {
                _logger.LogWarning("Invalid gradeId: {gradeId}", gradeId);
                return BadRequest("Invalid grade ID.");
            }

            try
            {
                var gradeDto = await _gradeService.GetGradeDto(gradeId, [GradeEntities.Assessments]);

                var managmentAssessmentDto = gradeDto.AssessmentDtoList.FirstOrDefault(a => a.SystemAssessmentType == SystemAssessmentTypes.ManagementCompetencies);

                if (managmentAssessmentDto == null)
                {
                    throw new Exception($"Managment competencies assessment is null for Grade with ID {gradeId}.");
                }

                var assessmentSummaryDto = await _assessmentService.GetAssessmentSummary(managmentAssessmentDto.Id);

                if (assessmentSummaryDto == null)
                {
                    throw new Exception($"Managment competencies assessment summary is null for Grade with ID {gradeId}.");
                }

                var viewModel = new ManagmentCompetenciesViewModel
                {
                    Conclusion = gradeDto.ManagmentCompetenciesConclusion,
                    AssessmentSummaryDto = assessmentSummaryDto,
                };

                return View("_ManagmentCompetenciesPartial", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AssessmentController.GetManagmentCompetenciesPopup] : ");

                return View("Error", new ErrorViewModel
                {
                    StatusCode = StatusCodes.InternalServerError,
                    Message = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetLastAssessments(int userId)
        {
            try
            {
                var userLastGradeAssessmentsDtos = await _userService.GetUserLastGradeAssessmentDtoList(userId);

                return Json(new { success = true, data = userLastGradeAssessmentsDtos });
            }

            catch
            {
                // LOG!!!
                return Json(new { success = false, message = "An unexpected error occurred. Please try again later." });
            }
        }
    }
}