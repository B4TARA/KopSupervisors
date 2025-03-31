using System.Security.Claims;
using KOP.BLL.Interfaces;
using KOP.Common.Dtos.AssessmentDtos;
using KOP.Common.Enums;
using KOP.WEB.Models.RequestModels;
using KOP.WEB.Models.ViewModels;
using KOP.WEB.Models.ViewModels.Employee;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        [Authorize]
        public IActionResult GetEmployeeLayout()
        {
            var currentUserId = Convert.ToInt32(User.FindFirstValue("Id"));
            var currentUserFullName = User.FindFirstValue("FullName") ?? "-";

            HttpContext.Session.SetString("SelectedUserFullName", currentUserFullName);
            HttpContext.Session.SetInt32("SelectedUserId", currentUserId);

            return View("EmployeeLayout", currentUserId);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAssessmentLayout()
        {
            var currentUserId = Convert.ToInt32(User.FindFirstValue("Id"));

            var isActive = await _assessmentService.IsActiveAssessment(currentUserId, currentUserId);

            var viewModel = new AssessmentLayoutViewModel
            {
                UserId = currentUserId,
                IsActiveSelfAssessment = isActive
            };

            return PartialView("_AssessmentLayoutPartial", viewModel);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetGradeLayout()
        {
            var currentUserId = Convert.ToInt32(User.FindFirstValue("Id"));

            var userDto = await _userService.GetUser(currentUserId);

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
                return PartialView("_GradeLayoutPartial", viewModel);
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

            return View("_GradeLayoutPartial", viewModel);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetColleaguesAssessment()
        {
            var currentUserId = Convert.ToInt32(User.FindFirstValue("Id"));

            var response = await _userService.GetColleaguesAssessmentResultsForAssessment(currentUserId);

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

            return PartialView("_ColleaguesAssessmentPartial", viewModel);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetSelfAssessment(int assessmentId)
        {
            try
            {
                var assessmentSummaryDto = await _assessmentService.GetAssessmentSummary(assessmentId);

                return PartialView("_SelfAssessmentPartial", assessmentSummaryDto);
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
        [Authorize]
        public async Task<IActionResult> GetSelfAssessmentLayout()
        {
            var currentUserId = Convert.ToInt32(User.FindFirstValue("Id"));

            var lastAssessmentOfEachType = await _userService.GetUserLastAssessmentsOfEachAssessmentType(currentUserId, currentUserId);

            var viewModel = new SelfAssessmentLayoutViewModel
            {
                LastAssessmentDtoList = lastAssessmentOfEachType,
            };

            return PartialView("_SelfAssessmentLayoutPartial", viewModel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AssessUser([FromBody] AssessUserRequestModel requestModel)
        {
            var assessUserDTO = new AssessUserDto
            {
                ResultValues = requestModel.resultValues,
                AssessmentResultId = requestModel.assessmentResultId,
            };

            await _userService.AssessUser(assessUserDTO);

            return StatusCode(200);
        }

        [HttpPost]
        [Authorize]
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
        public async Task<IActionResult> GetGrades(int userId)
        {
            try
            {
                var gradeSummaryDtoList = await _userService.GetUserGradesSummaries(userId);

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