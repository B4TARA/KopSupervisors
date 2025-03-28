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
        public async Task<IActionResult> GetCorporateCompetenciesPopup(int employeeId, int gradeId)
        {
            try
            {
                var lastAssessmentIdForUserAndTypeRes = await _userService.GetLastAssessmentIdForUserAndType(employeeId, SystemAssessmentTypes.СorporateСompetencies);

                if (!lastAssessmentIdForUserAndTypeRes.HasData)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = lastAssessmentIdForUserAndTypeRes.StatusCode,
                        Message = lastAssessmentIdForUserAndTypeRes.Description,
                    });
                }

                var assessmentSummaryDto = await _assessmentService.GetAssessmentSummary(lastAssessmentIdForUserAndTypeRes.Data);
                var gradeDto = await _gradeService.GetGradeDto(gradeId, new List<GradeEntities>());

                var viewModel = new CorporateCompetenciesViewModel
                {
                    Conclusion = gradeDto.CorporateCompetenciesConclusion,
                    AssessmentSummaryDto = assessmentSummaryDto,
                };

                return View("_CorporateCompetenciesPartial", viewModel);
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

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetManagmentCompetenciesPopup(int employeeId, int gradeId)
        {
            try
            {
                var lastAssessmentIdForUserAndTypeRes = await _userService.GetLastAssessmentIdForUserAndType(employeeId, SystemAssessmentTypes.ManagementCompetencies);

                if (!lastAssessmentIdForUserAndTypeRes.HasData)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = lastAssessmentIdForUserAndTypeRes.StatusCode,
                        Message = lastAssessmentIdForUserAndTypeRes.Description,
                    });
                }

                var assessmentSummaryDto = await _assessmentService.GetAssessmentSummary(lastAssessmentIdForUserAndTypeRes.Data);
                var gradeDto = await _gradeService.GetGradeDto(gradeId, new List<GradeEntities>());

                var viewModel = new ManagmentCompetenciesViewModel
                {
                    Conclusion = gradeDto.ManagmentCompetenciesConclusion,
                    AssessmentSummaryDto = assessmentSummaryDto,
                };

                return View("_ManagmentCompetenciesPartial", viewModel);
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

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetLastAssessments(int userId)
        {
            try
            {
                var lastAssessmentsOfEachType = await _userService.GetUserLastAssessmentsOfEachAssessmentType(userId, userId);

                return Json(new { success = true, data = lastAssessmentsOfEachType });
            }

            catch
            {
                // LOG!!!
                return Json(new { success = false, message = "An unexpected error occurred. Please try again later." });
            }
        }
    }
}