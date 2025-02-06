using System.Security.Claims;
using KOP.BLL.Interfaces;
using KOP.Common.Dtos.AssessmentDtos;
using KOP.WEB.Models.RequestModels;
using KOP.WEB.Models.ViewModels;
using KOP.WEB.Models.ViewModels.Employee;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StatusCodes = KOP.Common.Enums.StatusCodes;

namespace KOP.WEB.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IUserService _userService;
        private readonly IAssessmentService _assessmentService;

        public EmployeeController(IUserService userService, IAssessmentService assessmentService)
        {
            _userService = userService;
            _assessmentService = assessmentService;
        }

        [HttpGet]
        [Authorize(Roles = "Employee")]
        public IActionResult GetEmployeeLayout()
        {
            var currentUserId = Convert.ToInt32(User.FindFirstValue("Id"));

            return View("EmployeeLayout", currentUserId);
        }

        [HttpGet]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> GetAssessmentLayout(int userId)
        {
            var response = await _assessmentService.IsActiveAssessment(userId, userId);

            if (response.StatusCode != StatusCodes.OK)
            {
                return View("Error", new ErrorViewModel
                {
                    StatusCode = response.StatusCode,
                    Message = response.Description,
                });
            }

            var viewModel = new AssessmentLayoutViewModel
            {
                EmployeeId = userId,
                IsActiveSelfAssessment = response.Data
            };

            return View("AssessmentLayout", viewModel);
        }

        [HttpGet]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> GetGradeLayout(int employeeId)
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

            var viewModel = new GradeLayoutViewModel
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

            return View("GradeLayout", viewModel);
        }

        [HttpGet]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> GetColleaguesAssessment(int employeeId)
        {
            var response = await _userService.GetColleaguesAssessmentResultsForAssessment(employeeId);

            if (response.StatusCode != StatusCodes.OK)
            {
                return View("Error", new ErrorViewModel
                {
                    StatusCode = response.StatusCode,
                    Message = response.Description,
                });
            }

            var viewModel = new ColleagueAssessmentViewModel
            {
                ColleagueAssessmentResults = response.Data,
            };

            return View("ColleaguesAssessment", viewModel);
        }

        [HttpGet]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> GetSelfAssessment(int employeeId, int assessmentId)
        {
            var response = await _userService.GetUserSelfAssessmentResultByAssessment(employeeId, assessmentId);

            if (response.StatusCode != StatusCodes.OK)
            {
                return View("Error", new ErrorViewModel
                {
                    StatusCode = response.StatusCode,
                    Message = response.Description,
                });
            }

            var viewModel = new SelfAssessmentViewModel
            {
                SelfAssessmentResult = response.Data,
            };

            return View("SelfAssessment", viewModel);
        }

        [HttpGet]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> GetSelfAssessmentLayout(int employeeId)
        {
            var response = await _userService.GetUserLastAssessmentsOfEachAssessmentType(employeeId, employeeId);

            if (response.StatusCode != StatusCodes.OK)
            {
                return View("Error", new ErrorViewModel
                {
                    StatusCode = response.StatusCode,
                    Message = response.Description,
                });
            }

            var viewModel = new SelfAssessmentLayoutViewModel
            {
                LastAssessments = response.Data,
            };

            return View("SelfAssessmentLayout", viewModel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AssessEmployee([FromBody] AssessEmployeeRequestModel requestModel)
        {
            var assessEmployeeDTO = new AssessUserDto
            {
                ResultValues = requestModel.resultValues,
                AssessmentResultId = requestModel.assessmentResultId,
            };

            var response = await _userService.AssessUser(assessEmployeeDTO);

            if (response.StatusCode != StatusCodes.OK)
            {
                return StatusCode(Convert.ToInt32(response.StatusCode), new
                {
                    message = response.Description
                });
            }

            return StatusCode(200);
        }
    }
}