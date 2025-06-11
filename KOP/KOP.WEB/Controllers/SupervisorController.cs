using KOP.BLL.Interfaces;
using KOP.Common.Dtos;
using KOP.Common.Dtos.GradeDtos;
using KOP.Common.Enums;
using KOP.DAL.Interfaces;
using KOP.WEB.Models.RequestModels;
using KOP.WEB.Models.ViewModels;
using KOP.WEB.Models.ViewModels.Supervisor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using StatusCodes = KOP.Common.Enums.StatusCodes;

namespace KOP.WEB.Controllers
{
    public class SupervisorController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISupervisorService _supervisorService;
        private readonly IAssessmentService _assessmentService;
        private readonly IAssessmentResultService _assessmentResultService;
        private readonly IUserService _userService;
        private readonly ILogger<SupervisorController> _logger;

        public SupervisorController(IUnitOfWork unitOfWork, ISupervisorService supervisorService, IAssessmentService assessmentService,
            IAssessmentResultService assessmentResultService, IUserService userService, ILogger<SupervisorController> logger)
        {
            _unitOfWork = unitOfWork;
            _supervisorService = supervisorService;
            _assessmentService = assessmentService;
            _assessmentResultService = assessmentResultService;
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Supervisor, Urp, Curator, Uop, Umst, Cup")]
        public IActionResult GetSupervisorLayout()
        {
            try
            {
                var currentUserId = Convert.ToInt32(User.FindFirstValue("Id"));

                if (currentUserId <= 0)
                {
                    _logger.LogWarning("CurrentUserId is incorrect or not found in claims.");
                    return BadRequest("Current user ID is not valid.");
                }

                return View("SupervisorLayout", currentUserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[SupervisorController.GetSupervisorLayout] : ");

                return View("Error", new ErrorViewModel
                {
                    StatusCode = StatusCodes.InternalServerError,
                    Message = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Supervisor, Urp, Curator, Uop, Umst, Cup")]
        public async Task<IActionResult> GetSubordinates(int supervisorId)
        {
            if (supervisorId <= 0)
            {
                _logger.LogWarning("Invalid supervisorId: {supervisorId}", supervisorId);

                return BadRequest("Invalid supervisor ID.");
            }

            try
            {
                var subdivisions = await _supervisorService.GetSubdivisionsForSupervisor(supervisorId);

                var viewModel = new SubordinatesViewModel
                {
                    SupervisorId = supervisorId,
                    Subdivisions = subdivisions,
                };

                return PartialView("_SubordinatesPartial", viewModel);
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
        [Authorize(Roles = "Supervisor, Urp, Curator, Uop, Umst, Cup")]
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

                if (currentUserId <= 0)
                {
                    _logger.LogWarning("CurrentUserId is incorrect or not found in claims.");
                    return BadRequest("Current user ID is not valid.");
                }

                var isActiveAssessment = await _assessmentService.IsActiveAssessment(currentUserId, employeeId);

                var viewModel = new EmployeeViewModel
                {
                    EmployeeId = employeeId,
                    IsActiveAssessment = isActiveAssessment,
                };

                return PartialView("_EmployeeLayoutPartial", viewModel);
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
        [Authorize(Roles = "Supervisor, Urp, Curator, Uop, Umst, Cup")]
        public async Task<IActionResult> GetEmployeeGradeLayout(int employeeId)
        {
            if (employeeId <= 0)
            {
                _logger.LogWarning("Invalid employeeId: {employeeId}", employeeId);

                return BadRequest("Invalid employee ID.");
            }

            try
            {
                var currentUserId = Convert.ToInt32(User.FindFirstValue("Id"));

                if (currentUserId <= 0)
                {
                    _logger.LogWarning("CurrentUserId is incorrect or not found in claims.");
                    return BadRequest("Current user ID is not valid.");
                }

                var employee = await _userService.GetUser(employeeId);

                HttpContext.Session.SetString("SelectedUserFullName", employee.FullName);
                HttpContext.Session.SetInt32("SelectedUserId", employee.Id);

                if (employee.LastGrade?.GradeStatus != GradeStatuses.READY_FOR_SUPERVISOR_APPROVAL)
                {
                    return PartialView("_EmployeeGradeLayoutPartial", employee);
                }

                var supervisor = await _userService.GetFirstSupervisorForUser(employeeId);

                if (supervisor == null)
                {
                    return PartialView("_EmployeeGradeLayoutPartial", employee);
                }

                ViewBag.AccessForSupervisorApproval = (currentUserId == supervisor.Id) || User.IsInRole("Urp");

                return PartialView("_EmployeeGradeLayoutPartial", employee);
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
        [Authorize(Roles = "Supervisor, Urp, Curator, Uop")]
        public async Task<IActionResult> GetEmployeeAssessmentLayout(int employeeId)
        {
            if (employeeId <= 0)
            {
                _logger.LogWarning("Invalid employeeId: {employeeId}", employeeId);

                return BadRequest("Invalid employee ID.");
            }

            try
            {
                var assessments = await _userService.GetLastGradeAssessmentsForUser(employeeId);

                return PartialView("_EmployeeAssessmentLayoutPartial", assessments);
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
        [Authorize(Roles = "Supervisor, Urp, Curator, Uop")]
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

                if (currentUserId <= 0)
                {
                    _logger.LogWarning("CurrentUser Id is incorrect or not found in claims.");
                    return BadRequest("Current user ID is not valid.");
                }

                var assessment = await _assessmentService.GetAssessment(assessmentId);

                var assessmentResult = await _assessmentResultService.GetAssessmentResult(currentUserId, assessmentId);

                var userRoles = User.Claims
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value)
                    .ToList();

                var choosedCandidates = assessment.AllAssessmentResults
                    .Where(x => x.Type == AssessmentResultTypes.ColleagueAssessment)
                    .Select(candidate => new CandidateForJudgeDto
                    {
                        Id = candidate.Judge.Id,
                        AssessmentResultId = candidate.Id,
                        FullName = candidate.Judge.FullName,
                        HasJudged = candidate.SystemStatus == SystemStatuses.COMPLETED
                    })
                    .ToList();

                var userId = currentUserId;
                int? supervisorId;
                var requiredRoles = new HashSet<SystemRoles> { SystemRoles.Employee, SystemRoles.Supervisor, SystemRoles.Curator };

                var supervisorResult = assessment.AllAssessmentResults
                    .FirstOrDefault(x => x.Type == AssessmentResultTypes.SupervisorAssessment);

                if (supervisorResult == null)
                {
                    var supervisorForCurrentUser = await _userService.GetFirstSupervisorForUser(userId);
                    supervisorId = supervisorForCurrentUser?.Id;
                }
                else
                {
                    supervisorId = supervisorResult.Judge.Id;
                }

                var candidatesForJudges = await _unitOfWork.Users.GetAllAsync(x =>
                    x.SystemRoles.Any(r => requiredRoles.Contains(r)) &&               
                    x.Id != supervisorId &&
                    x.Id != userId);

                var allCandidates = candidatesForJudges.Select(candidate => new CandidateForJudgeDto
                {
                    Id = candidate.Id,
                    FullName = candidate.FullName,
                }).ToList();

                var remainingCandidates = allCandidates
                    .Where(c => !choosedCandidates.Select(c => c.Id).Contains(c.Id))
                    .OrderBy(x => x.FullName)
                    .ToList();

                var viewModel = new EmployeeAssessmentViewModel
                {
                    Assessment = assessment,
                    ChooseJudgesAccess = _userService.CanChooseJudges(userRoles, assessment),
                    ChoosedCandidatesForJudges = choosedCandidates,
                    CandidatesForJudges = remainingCandidates,
                    SupervisorAssessmentResult = assessmentResult,
                };

                return PartialView("_EmployeeAssessmentPartial", viewModel);
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
        [Authorize(Roles = "Supervisor, Urp, Curator, Uop, Umst, Cup")]
        public async Task<IActionResult> GetAnalyticsLayout()
        {
            try
            {
                var currentUserId = Convert.ToInt32(User.FindFirstValue("Id"));

                if (currentUserId <= 0)
                {
                    _logger.LogWarning("CurrentUserId is incorrect or not found in claims.");
                    return BadRequest("Current user ID is not valid.");
                }

                var users = await _supervisorService.GetUsersWithAnyGradeForSupervisor(currentUserId);

                return View("AnalyticsLayout", users);
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
        [Authorize(Roles = "Supervisor, Urp, Curator, Uop, Umst, Cup")]
        public async Task<IActionResult> GetGradesReportLayout()
        {
            try
            {
                var currentUserId = Convert.ToInt32(User.FindFirstValue("Id"));

                if (currentUserId <= 0)
                {
                    _logger.LogWarning("CurrentUserId is incorrect or not found in claims.");
                    return BadRequest("Current user ID is not valid.");
                }

                var users = await _supervisorService.GetUsersWithAnyGradeForSupervisor(currentUserId);

                return View("GradesReportLayout", users);
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

        [HttpGet]
        [Authorize(Roles = "Supervisor, Urp, Curator, Uop, Umst, Cup")]
        public async Task<IActionResult> GetUpcomingGradesReport()
        {
            try
            {
                var currentUserId = Convert.ToInt32(User.FindFirstValue("Id"));

                if (currentUserId <= 0)
                {
                    _logger.LogWarning("CurrentUserId is incorrect or not found in claims.");
                    return BadRequest("Current user ID is not valid.");
                }

                var users = await _supervisorService.GetUsersWithAnyUpcomingGradeForSupervisor(currentUserId);

                return View("UpcomingGradesReport", users);
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
        [Authorize(Roles = "Supervisor, Urp, Curator")]
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

                if (currentUserId <= 0)
                {
                    _logger.LogWarning("CurrentUserId is incorrect or not found in claims.");
                    return BadRequest("Current user ID is not valid.");
                }

                var judgesIds = JsonConvert.DeserializeObject<List<string>>(requestModel.judgesIds);

                if (judgesIds == null)
                {
                    throw new Exception($"JudgesIds is null for Assessment with ID {requestModel.assessmentId}.");
                }

                foreach (var judgeId in judgesIds)
                {
                    await _assessmentResultService.CreatePendingColleagueAssessmentResult(Convert.ToInt32(judgeId), requestModel.assessmentId, currentUserId);
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
        [Authorize(Roles = "Supervisor, Urp, Curator")]
        public async Task<IActionResult> DeleteJudge([FromBody] int assessmentResultId)
        {
            if (assessmentResultId <= 0)
            {
                _logger.LogWarning("Invalid assessmentResultId: {assessmentResultId}", assessmentResultId);

                return BadRequest("Invalid assessmentResult ID.");
            }

            try
            {
                await _assessmentResultService.DeletePendingAssessmentResult(assessmentResultId);

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
        [Authorize(Roles = "Supervisor, Urp, Curator")]
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

        [HttpPost]
        [Authorize(Roles = "Urp")]
        public async Task<IActionResult> SuspendUser([FromBody] int userId)
        {
            if (userId <= 0)
            {
                _logger.LogWarning("Invalid gradeId: {userId}", userId);

                return BadRequest("Invalid user ID.");
            }

            try
            {
                await _supervisorService.SuspendUser(userId);

                return Ok("Операция увольнения/перевода успешно завершена");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[SupervisorController.SuspendUser({userId})] : ", userId);

                return BadRequest(new
                {
                    error = "Произошла ошибка при завершении оценки.",
                });
            }
        }
    }
}