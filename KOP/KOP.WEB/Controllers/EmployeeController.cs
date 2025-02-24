using System.Security.Claims;
using KOP.BLL.Interfaces;
using KOP.Common.Dtos.AssessmentDtos;
using KOP.Common.Enums;
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
            var getUserRes = await _userService.GetUser(employeeId);

            if (!getUserRes.HasData)
            {
                return View("Error", new ErrorViewModel
                {
                    StatusCode = getUserRes.StatusCode,
                    Message = getUserRes.Description,
                });
            }

            var user = getUserRes.Data;
            var viewModel = new GradeLayoutViewModel
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

            if (user.LastGrade is null)
            {
                viewModel.GradeStatus = GradeStatuses.GRADE_NOT_FOUND;
                return View("GradeLayout", viewModel);
            }

            viewModel.GradeStatus = user.LastGrade.GradeStatus;

            foreach (var dto in user.LastGrade.AssessmentDtos)
            {
                var getAssessmentSummaryRes = await _assessmentService.GetAssessmentSummary(dto.Id);
                if (!getAssessmentSummaryRes.HasData)
                {
                    continue;
                }

                var assessmentSummary = getAssessmentSummaryRes.Data;
                if (dto.SystemAssessmentType == SystemAssessmentTypes.СorporateСompetencies)
                {
                    viewModel.IsCorporateCompetenciesFinalized = assessmentSummary.IsFinalized;
                }
                else if (dto.SystemAssessmentType == SystemAssessmentTypes.ManagementCompetencies)
                {
                    viewModel.IsManagmentCompetenciesFinalized = assessmentSummary.IsFinalized;
                }
            }

            return View("GradeLayout", viewModel);
        }

        [HttpGet]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> GetColleaguesAssessment(int employeeId)
        {
            var response = await _userService.GetColleaguesAssessmentResultsForAssessment(employeeId);

            if (!response.HasData)
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

            if (!response.HasData)
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

            if (!response.HasData)
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
        public async Task<IActionResult> AssessEmployee([FromBody] AssessEmployeeRequestModel requestModel)
        {
            var assessEmployeeDTO = new AssessUserDto
            {
                ResultValues = requestModel.resultValues,
                AssessmentResultId = requestModel.assessmentResultId,
            };

            var response = await _userService.AssessUser(assessEmployeeDTO);

            if (!response.IsSuccess)
            {
                return StatusCode(Convert.ToInt32(response.StatusCode), new
                {
                    message = response.Description
                });
            }

            return StatusCode(200);
        }

        [HttpPost]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> ApproveGrade([FromBody] int gradeId)
        {
            try
            {
                var approveGradeRes = await _userService.ApproveGrade(gradeId);
                if (!approveGradeRes.IsSuccess)
                {
                    return BadRequest(new
                    {
                        error = "Произошла ошибка при завершении оценки.",
                        details = approveGradeRes.Description,
                    });
                }

                return Ok("Оценка успешно завершена");
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    error = "Произошла ошибка при завершении оценки.",
                    details = ex.Message
                });
            }
        }
    }
}