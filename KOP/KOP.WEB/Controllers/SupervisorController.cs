using KOP.BLL.Interfaces;
using KOP.Common.Enums;
using KOP.WEB.Models.ViewModels;
using KOP.WEB.Models.ViewModels.Supervisor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using StatusCodes = KOP.Common.Enums.StatusCodes;

namespace KOP.WEB.Controllers
{
    public class SupervisorController : Controller
    {
        private readonly ISupervisorService _supervisorService;
        private readonly IAssessmentService _assessmentService;
        private readonly IEmployeeService _employeeService;

        public SupervisorController(ISupervisorService supervisorService, IAssessmentService assessmentService, IEmployeeService employeeService)
        {
            _supervisorService = supervisorService;
            _assessmentService = assessmentService;
            _employeeService = employeeService;
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
                var response = await _supervisorService.GetAllSubordinateModules(supervisorId);

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
                    Modules = response.Data,
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
                var response = await _employeeService.GetEmployee(employeeId);

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
                    Subdivision = response.Data.Subdivision,
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
        [Authorize(Roles = "Supervisor, Curator, Umst, Cup, Urp, Uop")]
        public async Task<IActionResult> GetEmployeeAssessmentLayout(int employeeId)
        {
            try
            {
                var id = Convert.ToInt32(User.FindFirstValue("Id"));

                var response = await _employeeService.GetEmployeeLastAssessments(employeeId, id);

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
        [Authorize(Roles = "Supervisor, Curator, Umst, Cup, Urp, Uop")]
        public async Task<IActionResult> GetEmployeeAssessment(int employeeId)
        {
            try
            {
                var viewModel = new EmployeeAssessmentViewModel();

                var id = Convert.ToInt32(User.FindFirstValue("Id"));

                var response1 = await _assessmentService.GetAssessment(employeeId, SystemStatuses.COMPLETED);

                if (!response1.HasData)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = response1.StatusCode,
                        Message = response1.Description,
                    });
                }

                viewModel.Assessment = response1.Data;

                var response2 = await _assessmentService.GetAssessmentResult(id, employeeId);

                if (response2.HasData)
                {
                    viewModel.SupervisorAssessmentResult = response2.Data;
                }

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