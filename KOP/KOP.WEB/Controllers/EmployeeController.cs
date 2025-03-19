using System.Security.Claims;
using KOP.BLL.Interfaces;
using KOP.BLL.Services;
using KOP.Common.Dtos.AssessmentDtos;
using KOP.Common.Enums;
using KOP.WEB.Models.RequestModels;
using KOP.WEB.Models.ViewModels;
using KOP.WEB.Models.ViewModels.Employee;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPOI.OpenXmlFormats.Wordprocessing;

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
            var isActive = await _assessmentService.IsActiveAssessment(userId, userId);

            var viewModel = new AssessmentLayoutViewModel
            {
                EmployeeId = userId,
                IsActiveSelfAssessment = isActive
            };

            return View("AssessmentLayout", viewModel);
        }

        [HttpGet]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> GetGradeLayout(int employeeId)
        {
            var userDto = await _userService.GetUser(employeeId);

            var viewModel = new GradeLayoutViewModel
            {
                Id = userDto.Id,
                FullName = userDto.FullName,
                Position = userDto.Position,
                SubdivisionFromFile = userDto.SubdivisionFromFile,
                GradeGroup = userDto.GradeGroup,
                WorkPeriod = userDto.WorkPeriod,
                ContractEndDate = userDto.ContractEndDate,
                ImagePath = userDto.ImagePath,
                LastGradeDto = userDto.LastGrade,
            };

            if (userDto.LastGrade == null)
            {
                viewModel.GradeStatus = GradeStatuses.GRADE_NOT_FOUND;
                return View("GradeLayout", viewModel);
            }

            viewModel.GradeStatus = userDto.LastGrade.GradeStatus;

            foreach (var dto in userDto.LastGrade.AssessmentDtoList)
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
                ColleagueAssessmentResultDtoList = response.Data,
            };

            return View("ColleaguesAssessment", viewModel);
        }

        [HttpGet]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> GetSelfAssessment(int assessmentId)
        {
            try
            {
                var assessmentSummaryDto = await _assessmentService.GetAssessmentSummary(assessmentId);

                return View("SelfAssessment", assessmentSummaryDto);
            }
            catch
            {
                // LOG!!!
                return View("Error", new ErrorViewModel
                {
                    StatusCode = Common.Enums.StatusCodes.InternalServerError,
                    Message = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> GetSelfAssessmentLayout()
        {
            var currentUserId = Convert.ToInt32(User.FindFirstValue("Id"));

            var lastAssessmentOfEachType = await _userService.GetUserLastAssessmentsOfEachAssessmentType(currentUserId, currentUserId);

            var viewModel = new SelfAssessmentLayoutViewModel
            {
                LastAssessmentDtoList = lastAssessmentOfEachType,
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

            await _userService.AssessUser(assessEmployeeDTO);

            return StatusCode(200);
        }

        [HttpPost]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> ApproveGrade([FromBody] int gradeId)
        {
            try
            {
                await _userService.ApproveGrade(gradeId);

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

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetGrades(int employeeId)
        {
            try
            {
                var gradeSummaryDtoList = await _userService.GetUserGradesSummaries(employeeId);

                return View("EmployeeGrades", gradeSummaryDtoList);
            }
            catch
            {
                return View("Error", new ErrorViewModel
                {
                    StatusCode = Common.Enums.StatusCodes.InternalServerError,
                    Message = "An unexpected error occurred. Please try again later."
                });
            }
        }
    }
}