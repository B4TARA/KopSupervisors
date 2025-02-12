using System.Security.Claims;
using KOP.BLL.Interfaces;
using KOP.WEB.Models.ViewModels;
using KOP.WEB.Models.ViewModels.Supervisor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StatusCodes = KOP.Common.Enums.StatusCodes;

namespace KOP.WEB.Controllers
{
    public class SupervisorController : Controller
    {
        private readonly ISupervisorService _supervisorService;
        private readonly IAssessmentService _assessmentService;
        private readonly IUserService _userService;

        public SupervisorController(ISupervisorService supervisorService, IAssessmentService assessmentService, IUserService userService)
        {
            _supervisorService = supervisorService;
            _assessmentService = assessmentService;
            _userService = userService;
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
        public async Task<IActionResult> GetSubordinates(int supervisorId, bool onlyPending = false)
        {
            try
            {
                var response = await _supervisorService.GetUserSubordinateSubdivisions(supervisorId, onlyPending);

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
                var id = Convert.ToInt32(User.FindFirstValue("Id"));

                var response = await _assessmentService.IsActiveAssessment(id, employeeId);

                if (response.StatusCode != StatusCodes.OK)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = response.StatusCode,
                        Message = response.Description,
                    });
                }

                var viewModel = new EmployeeViewModel
                {
                    EmployeeId = employeeId,
                    IsActiveAssessment = response.Data,
                };

                return View("EmployeeLayout", viewModel);
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
        public async Task<IActionResult> GetEmployeeGradeLayout(int employeeId)
        {
            try
            {
                var response = await _userService.GetUser(employeeId);

                if (response.StatusCode != StatusCodes.OK || response.Data == null)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = response.StatusCode,
                        Message = response.Description,
                    });
                }

                var viewModel = new EmployeeGradeLayoutViewModel
                {
                    Id = response.Data.Id,
                    FullName = response.Data.FullName,
                    Position = response.Data.Position,
                    SubdivisionFromFile = response.Data.SubdivisionFromFile,
                    GradeGroup = response.Data.GradeGroup,
                    WorkPeriod = response.Data.WorkPeriod,
                    ContractEndDate = response.Data.ContractEndDate,
                    ImagePath = response.Data.ImagePath,
                    LastGrade = response.Data.LastGrade,
                };

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
                var id = Convert.ToInt32(User.FindFirstValue("Id"));

                var response = await _userService.GetUserLastAssessmentsOfEachAssessmentType(employeeId, id);

                if (response.StatusCode != StatusCodes.OK || response.Data == null)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = response.StatusCode,
                        Message = response.Description,
                    });
                }

                var viewModel = new EmployeeAssessmentLayoutViewModel
                {
                    LastAssessments = response.Data,
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
                var getAssessmentRes = await _assessmentService.GetAssessment(assessmentId);

                if (!getAssessmentRes.HasData)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = getAssessmentRes.StatusCode,
                        Message = getAssessmentRes.Description,
                    });
                }

                var viewModel = new EmployeeAssessmentViewModel
                {
                    Assessment = getAssessmentRes.Data
                };

                var getAssessmentResultRes = await _assessmentService.GetAssessmentResult(Convert.ToInt32(User.FindFirstValue("Id")), assessmentId);

                if (getAssessmentResultRes.HasData)
                {
                    viewModel.SupervisorAssessmentResult = getAssessmentResultRes.Data;
                }

                var userRoles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);
                viewModel.ChooseJudgesAccess = _userService.CanChooseJudges(userRoles, getAssessmentRes.Data);
                viewModel.CandidatesForJudges = await _userService.GetCandidatesForJudges();
                viewModel.ChoosedCandidatesForJudges = _userService.GetChoosedCandidatesForJudges(getAssessmentRes.Data.AssessmentResults);

                return View("EmployeeAssessment", viewModel);
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