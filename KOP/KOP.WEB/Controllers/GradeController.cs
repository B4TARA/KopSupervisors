using KOP.BLL.Interfaces;
using KOP.Common.Enums;
using KOP.WEB.Models.ViewModels;
using KOP.WEB.Models.ViewModels.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StatusCodes = KOP.Common.Enums.StatusCodes;

namespace KOP.WEB.Controllers
{
    public class GradeController : Controller
    {
        private readonly IGradeService _gradeService;
        private readonly IUserService _userService;
        private readonly IAssessmentService _assessmentService;

        public GradeController(IGradeService gradeService, IUserService userService, IAssessmentService assessmentService)
        {
            _gradeService = gradeService;
            _userService = userService;
            _assessmentService = assessmentService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetTrainingEventsPopup(int gradeId)
        {
            try
            {
                var gradeRes = await _gradeService.GetGrade(gradeId, new List<GradeEntities> { GradeEntities.TrainingEvents });

                if (!gradeRes.HasData)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = gradeRes.StatusCode,
                        Message = gradeRes.Description,
                    });
                }

                var viewModel = new TrainingEventsViewModel
                {
                    GradeId = gradeId,
                    TrainingEvents = gradeRes.Data.TrainingEvents,
                };

                return View("_TrainingEvents", viewModel);
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
        public async Task<IActionResult> GetCorporateCompetenciesPopup(int employeeId, int gradeId)
        {
            try
            {
                var lastAssessmentForUserAndTypeRes = await _userService.GetLastAssessmentForUserAndType(employeeId, SystemAssessmentTypes.СorporateСompetencies);

                if (!lastAssessmentForUserAndTypeRes.HasData)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = lastAssessmentForUserAndTypeRes.StatusCode,
                        Message = lastAssessmentForUserAndTypeRes.Description,
                    });
                }

                var assessmentSummaryRes = await _assessmentService.GetAssessmentSummary(lastAssessmentForUserAndTypeRes.Data.Id);

                if (!lastAssessmentForUserAndTypeRes.HasData)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = assessmentSummaryRes.StatusCode,
                        Message = assessmentSummaryRes.Description,
                    });
                }

                var gradeRes = await _gradeService.GetGrade(gradeId, new List<GradeEntities>());

                if (!gradeRes.HasData)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = gradeRes.StatusCode,
                        Message = gradeRes.Description,
                    });
                }

                var viewModel = new CorporateCompetenciesViewModel
                {
                    Conclusion = gradeRes.Data.CorporateCompetenciesConclusion,
                    AssessmentSummaryDto = assessmentSummaryRes.Data,
                };

                return View("_CorporateCompetencies", viewModel);
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
                var lastAssessmentForUserAndTypeRes = await _userService.GetLastAssessmentForUserAndType(employeeId, SystemAssessmentTypes.ManagementCompetencies);

                if (!lastAssessmentForUserAndTypeRes.HasData)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = lastAssessmentForUserAndTypeRes.StatusCode,
                        Message = lastAssessmentForUserAndTypeRes.Description,
                    });
                }

                var assessmentSummaryRes = await _assessmentService.GetAssessmentSummary(lastAssessmentForUserAndTypeRes.Data.Id);

                if (!lastAssessmentForUserAndTypeRes.HasData)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = assessmentSummaryRes.StatusCode,
                        Message = assessmentSummaryRes.Description,
                    });
                }

                var gradeRes = await _gradeService.GetGrade(gradeId, new List<GradeEntities>());

                if (!gradeRes.HasData)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = gradeRes.StatusCode,
                        Message = gradeRes.Description,
                    });
                }

                var viewModel = new ManagmentCompetenciesViewModel
                {
                    Conclusion = gradeRes.Data.ManagmentCompetenciesConclusion,
                    AssessmentSummaryDto = assessmentSummaryRes.Data,
                };

                return View("_ManagmentCompetencies", viewModel);
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
    }
}