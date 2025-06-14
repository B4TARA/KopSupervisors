﻿using KOP.BLL.Interfaces;
using KOP.Common.Dtos.AssessmentDtos;
using KOP.Common.Enums;
using KOP.WEB.Models.RequestModels;
using KOP.WEB.Models.ViewModels;
using KOP.WEB.Models.ViewModels.Employee;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
            var currentUserFullName = User.FindFirstValue("FullName") ?? "-";

            var viewModel = new EmployeeLayoutViewModel
            {
                Id = currentUserId,
                FullName = currentUserFullName,
            };

            HttpContext.Session.SetString("SelectedUserFullName", currentUserFullName);
            HttpContext.Session.SetInt32("SelectedUserId", currentUserId);

            return View("EmployeeLayout", viewModel);
        }

        [HttpGet]
        [Authorize(Roles = "Employee")]
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
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> GetGradeLayout()
        {
            var currentUserId = Convert.ToInt32(User.FindFirstValue("Id"));
            var currentUser = await _userService.GetUser(currentUserId);   

            return View("_GradeLayoutPartial", currentUser);
        }

        [HttpGet]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> GetColleaguesAssessmentResultsForAssessment()
        {
            var currentUserId = Convert.ToInt32(User.FindFirstValue("Id"));
            var assessmentResultDtoList = await _userService.GetColleaguesAssessmentResultsForAssessment(currentUserId);

            var viewModel = new ColleagueAssessmentViewModel
            {
                ColleagueAssessmentResultDtoList = assessmentResultDtoList,
            };

            return PartialView("_ColleaguesAssessmentPartial", viewModel);
        }

        [HttpGet]
        [Authorize(Roles = "Employee")]
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
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> GetSelfAssessmentLayout()
        {
            var currentUserId = Convert.ToInt32(User.FindFirstValue("Id"));
            var assessments = await _userService.GetLastGradeAssessmentsForUser(currentUserId);

            var viewModel = new SelfAssessmentLayoutViewModel
            {
                LastGradeAssessmentDtoList = assessments,
            };

            return PartialView("_SelfAssessmentLayoutPartial", viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Supervisor, Urp, Curator, Employee")]
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
        [Authorize(Roles = "Supervisor, Urp, Curator, Uop, Umst, Cup")]
        public async Task<IActionResult> GetGrades(int userId)
        {
            try
            {
                var grades = await _userService.GetGradesForUser(userId);

                return View("EmployeeGrades", grades);
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