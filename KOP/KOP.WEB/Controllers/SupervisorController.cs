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

        public SupervisorController(ISupervisorService supervisorService, IAssessmentService assessmentService,
            IUserService userService, ICommonService commonService)
        {
            _supervisorService = supervisorService;
            _assessmentService = assessmentService;
            _userService = userService;
            _commonService = commonService;
        }

        [HttpGet]
        [Authorize(Roles = "Supervisor, Curator, Umst, Cup, Urp, Uop")]
        public IActionResult GetSupervisorLayout()
        {
            try
            {
                var id = Convert.ToInt32(User.FindFirstValue("Id"));

                return View("SupervisorLayout", id);
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
        [Authorize(Roles = "Supervisor, Curator, Umst, Cup, Urp, Uop")]
        public async Task<IActionResult> GetSubordinates(int supervisorId)
        {
            try
            {
                var response = await _supervisorService.GetUserSubordinateSubdivisions(supervisorId);

                if (response.StatusCode != StatusCodes.OK || response.Data == null)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = response.StatusCode,
                        Message = response.Description,
                    });
                }

                var viewModel = new SubordinatesViewModel
                {
                    SupervisorId = supervisorId,
                    Subdivisions = response.Data.ToList(),
                };

                return View("Subordinates", viewModel);
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
        [Authorize(Roles = "Supervisor, Curator, Umst, Cup, Urp, Uop")]
        public async Task<IActionResult> GetEmployeeLayout(int employeeId)
        {
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
                // Логирование ошибки
                //_logger.LogError(ex, "An error occurred while getting the employee layout for employee ID {EmployeeId}.", employeeId);

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

                if (user.LastGrade == null)
                {
                    viewModel.GradeStatus = GradeStatuses.GRADE_NOT_FOUND;

                    return View("EmployeeGradeLayout", viewModel);
                }

                viewModel.GradeStatus = user.LastGrade.GradeStatus;

                foreach (var dto in user.LastGrade.AssessmentDtos)
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
        [Authorize(Roles = "Supervisor, Curator, Urp, Uop")]
        public async Task<IActionResult> GetEmployeeAssessmentLayout(int employeeId)
        {
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
        [Authorize(Roles = "Supervisor, Curator, Urp, Uop")]
        public async Task<IActionResult> GetEmployeeAssessment(int assessmentId)
        {
            try
            {
                var currentUserId = Convert.ToInt32(User.FindFirstValue("Id"));
                var assessment = await _assessmentService.GetAssessment(assessmentId);
                var assessmentResult = await _assessmentService.GetAssessmentResult(currentUserId, assessmentId);

                var userRoles = User.Claims
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value);

                var choosedCandidates = await _userService.GetChoosedCandidatesForJudges(assessment.AllAssessmentResults, assessment.UserId);
                var allCandidates = await _userService.GetCandidatesForJudges(assessment.UserId);

                var remainingCandidates = allCandidates
                    .Where(c => !choosedCandidates.Select(c => c.Id).Contains(c.Id))
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
            catch
            {
                // Логирование ошибки
                // _logger.LogError(ex, "An error occurred while processing the assessment.");

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
            try
            {
                var judgesIds = JsonConvert.DeserializeObject<List<string>>(requestModel.judgesIds);
                var assessmentId = requestModel.assessmentId;
                var currentUserId = Convert.ToInt32(User.FindFirstValue("Id"));

                if (judgesIds.Count() > 3)
                {
                    return BadRequest(new
                    {
                        error = "Произошла ошибка при сохранении.",
                        details = "Число оценщиков должно быть меньше 3"
                    });
                }

                foreach (var judgeId in judgesIds)
                {
                    await _assessmentService.AddJudgeForAssessment(Convert.ToInt32(judgeId), assessmentId, currentUserId);
                }

                return Ok("Сохранение прошло успешно");
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
        [Authorize(Roles = "Supervisor, Urp")]
        public async Task<IActionResult> DeleteJudge([FromBody] int assessmentResultId)
        {
            try
            {
                await _assessmentService.DeleteJudgeForAssessment(assessmentResultId);       
                
                return Ok("Удаление прошло успешно");
            }
            catch
            {
                // Логирование ошибки
                // _logger.LogError(ex, "An error occurred while processing the assessment.");

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
            try
            {
                var approveEmployeeGradeRes = await _supervisorService.ApproveEmployeeGrade(gradeId);
                if (!approveEmployeeGradeRes.IsSuccess)
                {
                    return BadRequest(new
                    {
                        error = "Произошла ошибка при завершении оценки.",
                        details = approveEmployeeGradeRes.Description,
                    });
                }

                return Ok("Оценка успешно завершена");
            }
            catch
            {
                return BadRequest(new
                {
                    error = "Произошла ошибка при завершении оценки.",
                });
            }
        }
    }
}