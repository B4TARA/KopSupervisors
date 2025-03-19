using System.Security.Claims;
using KOP.BLL.Interfaces;
using KOP.Common.Enums;
using KOP.WEB.Models.RequestModels;
using KOP.WEB.Models.ViewModels;
using KOP.WEB.Models.ViewModels.Supervisor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StatusCodes = KOP.Common.Enums.StatusCodes;

namespace KOP.WEB.Controllers
{
    public class SupervisorController : Controller
    {
        private readonly ISupervisorService _supervisorService;
        private readonly IAssessmentService _assessmentService;
        private readonly IUserService _userService;
        private readonly ICommonService _commonService;
        private readonly ILogger<SupervisorController> _logger;

        public SupervisorController(ISupervisorService supervisorService, IAssessmentService assessmentService,
            IUserService userService, ICommonService commonService, ILogger<SupervisorController> logger)
        {
            _supervisorService = supervisorService;
            _assessmentService = assessmentService;
            _userService = userService;
            _commonService = commonService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Supervisor, Curator, Umst, Cup, Urp, Uop")]
        public IActionResult GetSupervisorLayout()
        {
            var id = Convert.ToInt32(User.FindFirstValue("Id"));

            return View("SupervisorLayout", id);
        }

        [HttpGet]
        [Authorize(Roles = "Supervisor, Curator, Umst, Cup, Urp, Uop")]
        public async Task<IActionResult> GetSubordinates(int supervisorId)
        {
            if (supervisorId <= 0)
            {
                _logger.LogWarning("Invalid supervisorId: {supervisorId}", supervisorId);

                return BadRequest("Invalid supervisor ID.");
            }

            try
            {
                var subordinateSubdivisionDtos = await _supervisorService.GetSubordinateSubdivisions(supervisorId);
                var subordinateSubdivisionDtoList = subordinateSubdivisionDtos.ToList();

                var viewModel = new SubordinatesViewModel
                {
                    SupervisorId = supervisorId,
                    Subdivisions = subordinateSubdivisionDtoList,
                };

                return View("Subordinates", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[SupervisorController.GetSubordinates({supervisorId})] : ", supervisorId);

                return View("Error", new ErrorViewModel
                {
                    StatusCode = StatusCodes.InternalServerError,
                    Message = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Supervisor, Curator, Umst, Cup, Urp, Uop")]
        public async Task<IActionResult> GetEmployeeLayout(int employeeId)
        {
            if (employeeId <= 0)
            {
                _logger.LogWarning("Invalid employeeId: {employeeId}", employeeId);

                return BadRequest("Invalid employee ID.");
            }

            try
            {
                var currentUserId = Convert.ToInt32(User.FindFirstValue("Id"));

                var isActiveAssessment = await _assessmentService.IsActiveAssessment(currentUserId, employeeId);

                var viewModel = new EmployeeViewModel
                {
                    EmployeeId = employeeId,
                    IsActiveAssessment = isActiveAssessment,
                };

                return View("EmployeeLayout", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[SupervisorController.GetEmployeeLayout({employeeId})] : ", employeeId);

                return View("Error", new ErrorViewModel
                {
                    StatusCode = StatusCodes.InternalServerError,
                    Message = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Supervisor, Curator, Umst, Cup, Urp, Uop")]
        public async Task<IActionResult> GetEmployeeGradeLayout(int employeeId)
        {
            if (employeeId <= 0)
            {
                _logger.LogWarning("Invalid employeeId: {employeeId}", employeeId);

                return BadRequest("Invalid employee ID.");
            }

            try
            {
                var user = await _userService.GetUser(employeeId);

                var viewModel = new EmployeeGradeLayoutViewModel
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Position = user.Position,
                    SubdivisionFromFile = user.SubdivisionFromFile,
                    GradeGroup = user.GradeGroup,
                    WorkPeriod = user.WorkPeriod,
                    ContractEndDate = user.ContractEndDate,
                    ImagePath = user.ImagePath,
                    LastGrade = user.LastGrade,
                };

                HttpContext.Session.SetString("SelectedUserFullName", viewModel.FullName);
                HttpContext.Session.SetInt32("SelectedUserId", viewModel.Id);

                if (user.LastGrade == null)
                {
                    viewModel.GradeStatus = GradeStatuses.GRADE_NOT_FOUND;

                    return View("EmployeeGradeLayout", viewModel);
                }

                viewModel.GradeStatus = user.LastGrade.GradeStatus;

                foreach (var dto in user.LastGrade.AssessmentDtoList)
                {
                    var assessmentSummaryDto = await _assessmentService.GetAssessmentSummary(dto.Id);

                    if (dto.SystemAssessmentType == SystemAssessmentTypes.СorporateСompetencies)
                    {
                        viewModel.IsCorporateCompetenciesFinalized = assessmentSummaryDto.IsFinalized;
                    }
                    else if (dto.SystemAssessmentType == SystemAssessmentTypes.ManagementCompetencies)
                    {
                        viewModel.IsManagmentCompetenciesFinalized = assessmentSummaryDto.IsFinalized;
                    }
                }

                if (user.LastGrade.GradeStatus != GradeStatuses.READY_FOR_SUPERVISOR_APPROVAL)
                {
                    return View("EmployeeGradeLayout", viewModel);
                }

                var supervisor = await _commonService.GetSupervisorForUser(employeeId);
                if (supervisor == null)
                {
                    return View("EmployeeGradeLayout", viewModel);
                }

                var currentUserId = Convert.ToInt32(User.FindFirstValue("Id"));
                viewModel.AccessForSupervisorApproval = (currentUserId == supervisor.Id) || User.IsInRole("Urp");

                return View("EmployeeGradeLayout", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[SupervisorController.GetEmployeeGradeLayout({employeeId})] : ", employeeId);

                return View("Error", new ErrorViewModel
                {
                    StatusCode = StatusCodes.InternalServerError,
                    Message = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Supervisor, Curator, Urp, Uop")]
        public async Task<IActionResult> GetEmployeeAssessmentLayout(int employeeId)
        {
            if (employeeId <= 0)
            {
                _logger.LogWarning("Invalid employeeId: {employeeId}", employeeId);

                return BadRequest("Invalid employee ID.");
            }

            try
            {
                var currentUserId = Convert.ToInt32(User.FindFirstValue("Id"));

                var lastAssessmentOfEachType = await _userService.GetUserLastAssessmentsOfEachAssessmentType(employeeId, currentUserId);

                var viewModel = new EmployeeAssessmentLayoutViewModel
                {
                    LastAssessments = lastAssessmentOfEachType,
                };

                return View("EmployeeAssessmentLayout", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[SupervisorController.GetEmployeeAssessmentLayout({employeeId})] : ", employeeId);

                return View("Error", new ErrorViewModel
                {
                    StatusCode = StatusCodes.InternalServerError,
                    Message = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Supervisor, Curator, Urp, Uop")]
        public async Task<IActionResult> GetEmployeeAssessment(int assessmentId)
        {
            if (assessmentId <= 0)
            {
                _logger.LogWarning("Invalid assessmentId: {assessmentId}", assessmentId);

                return BadRequest("Invalid assessment ID.");
            }

            try
            {
                var currentUserId = Convert.ToInt32(User.FindFirstValue("Id"));

                var assessment = await _assessmentService.GetAssessment(assessmentId);
                var assessmentResult = await _assessmentService.GetAssessmentResult(currentUserId, assessmentId);

                var userRoles = User.Claims
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value)
                    .ToList();

                var choosedCandidates = await _userService.GetChoosedCandidatesForJudges(assessment.AllAssessmentResults, assessment.UserId);
                var allCandidates = await _userService.GetCandidatesForJudges(assessment.UserId);

                var remainingCandidates = allCandidates
                    .Where(c => !choosedCandidates
                    .Select(c => c.Id)
                    .Contains(c.Id))
                    .OrderBy(x => x.FullName)
                    .ToList();

                var viewModel = new EmployeeAssessmentViewModel
                {
                    Assessment = assessment,
                    ChooseJudgesAccess = _userService.CanChooseJudges(userRoles, assessment),
                    ChoosedCandidatesForJudges = choosedCandidates.ToList(),
                    CandidatesForJudges = remainingCandidates,
                    SupervisorAssessmentResult = assessmentResult,
                };

                return View("EmployeeAssessment", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[SupervisorController.GetEmployeeAssessment({assessmentId})] : ", assessmentId);

                return View("Error", new ErrorViewModel
                {
                    StatusCode = StatusCodes.InternalServerError,
                    Message = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAnalyticsLayout()
        {
            try
            {
                var currentUserId = Convert.ToInt32(User.FindFirstValue("Id"));

                var subordinateUsersSummariesHasGrades = await _supervisorService.GetSubordinateUsersSummariesHasGrade(currentUserId);

                return View("AnalyticsLayout", subordinateUsersSummariesHasGrades);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[SupervisorController.GetAnalyticsLayout] : ");

                return View("Error", new ErrorViewModel
                {
                    StatusCode = StatusCodes.InternalServerError,
                    Message = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetReportLayout()
        {
            try
            {
                var currentUserId = Convert.ToInt32(User.FindFirstValue("Id"));

                var subordinateUsersSummariesHasGrades = await _supervisorService.GetSubordinateUsersSummariesHasGrade(currentUserId);

                return View("ReportLayout", subordinateUsersSummariesHasGrades);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[SupervisorController.GetReportLayout] : ");

                return View("Error", new ErrorViewModel
                {
                    StatusCode = StatusCodes.InternalServerError,
                    Message = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Supervisor, Urp")]
        public async Task<IActionResult> AddJudges(AddJudgesRequestModel requestModel)
        {
            if (requestModel.assessmentId <= 0)
            {
                _logger.LogWarning("Invalid assessmentId: {assessmentId}", requestModel.assessmentId);

                return BadRequest("Invalid assessment ID.");
            }

            try
            {
                var currentUserId = Convert.ToInt32(User.FindFirstValue("Id"));

                var assessmentId = requestModel.assessmentId;
                var judgesIds = JsonConvert.DeserializeObject<List<string>>(requestModel.judgesIds);

                if (judgesIds == null)
                {
                    throw new Exception($"JudgesIds is null for Assessment with ID {requestModel.assessmentId}.");
                }

                foreach (var judgeId in judgesIds)
                {
                    await _assessmentService.AddJudgeForAssessment(Convert.ToInt32(judgeId), assessmentId, currentUserId);
                }

                return Ok("Сохранение прошло успешно");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[SupervisorController.AddJudges({requestModel.assessmentId})] : ", requestModel.assessmentId);

                return BadRequest(new
                {
                    error = "Произошла ошибка при сохранении.",
                    details = ex.Message
                });
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Supervisor, Urp")]
        public async Task<IActionResult> DeleteJudge([FromBody] int assessmentResultId)
        {
            if (assessmentResultId <= 0)
            {
                _logger.LogWarning("Invalid assessmentResultId: {assessmentResultId}", assessmentResultId);

                return BadRequest("Invalid assessmentResult ID.");
            }

            try
            {
                await _assessmentService.DeleteJudgeForAssessment(assessmentResultId);

                return Ok("Удаление прошло успешно");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[SupervisorController.DeleteJudge({assessmentResultId})] : ", assessmentResultId);

                return BadRequest(new
                {
                    error = "Произошла ошибка при удалении.",
                });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Supervisor, Urp")]
        public async Task<IActionResult> ApproveEmployeeGrade([FromBody] int gradeId)
        {
            if (gradeId <= 0)
            {
                _logger.LogWarning("Invalid gradeId: {gradeId}", gradeId);

                return BadRequest("Invalid grade ID.");
            }

            try
            {
                await _supervisorService.ApproveGrade(gradeId);

                return Ok("Оценка успешно завершена");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[SupervisorController.ApproveEmployeeGrade({gradeId})] : ", gradeId);

                return BadRequest(new
                {
                    error = "Произошла ошибка при завершении оценки.",
                });
            }
        }
    }
}