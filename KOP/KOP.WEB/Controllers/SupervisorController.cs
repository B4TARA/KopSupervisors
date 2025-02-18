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
                var getUserRes = await _userService.GetUser(employeeId);

                if (!getUserRes.HasData)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = getUserRes.StatusCode,
                        Message = getUserRes.Description,
                    });
                }

                var viewModel = new EmployeeGradeLayoutViewModel
                {
                    Id = getUserRes.Data.Id,
                    FullName = getUserRes.Data.FullName,
                    Position = getUserRes.Data.Position,
                    SubdivisionFromFile = getUserRes.Data.SubdivisionFromFile,
                    GradeGroup = getUserRes.Data.GradeGroup,
                    WorkPeriod = getUserRes.Data.WorkPeriod,
                    ContractEndDate = getUserRes.Data.ContractEndDate,
                    ImagePath = getUserRes.Data.ImagePath,
                    LastGrade = getUserRes.Data.LastGrade,
                };

                var lastСorporateAssessmentIdRes = await _userService.GetLastAssessmentIdForUserAndType(employeeId, SystemAssessmentTypes.СorporateСompetencies);

                if (lastСorporateAssessmentIdRes.HasData)
                {
                    var lastСorporateAssessmentSummaryRes = await _assessmentService.GetAssessmentSummary(lastСorporateAssessmentIdRes.Data);

                    if (lastСorporateAssessmentSummaryRes.HasData)
                    {
                        viewModel.IsCorporateCompetenciesFinalized = lastСorporateAssessmentSummaryRes.Data.IsFinalized;
                    }
                }           

                var lastManagmentAssessmentIdRes = await _userService.GetLastAssessmentIdForUserAndType(employeeId, SystemAssessmentTypes.ManagementCompetencies);

                if (lastManagmentAssessmentIdRes.HasData)
                {
                    var lastManagmentAssessmentSummaryRes = await _assessmentService.GetAssessmentSummary(lastManagmentAssessmentIdRes.Data);

                    if (lastManagmentAssessmentSummaryRes.HasData)
                    {
                        viewModel.IsCorporateCompetenciesFinalized = lastManagmentAssessmentSummaryRes.Data.IsFinalized;
                    }
                }        

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
                var choosedCandidates = await _userService.GetChoosedCandidatesForJudges(getAssessmentRes.Data.AllAssessmentResults, getAssessmentRes.Data.UserId);
                var allCandidates = await _userService.GetCandidatesForJudges(getAssessmentRes.Data.UserId);
                var choosedCandidateIds = choosedCandidates.Select(c => c.Id).ToList();
                var remainingCandidates = allCandidates.Where(c => !choosedCandidateIds.Contains(c.Id)).ToList();

                viewModel.ChooseJudgesAccess = _userService.CanChooseJudges(userRoles, getAssessmentRes.Data);
                viewModel.ChoosedCandidatesForJudges = choosedCandidates;
                viewModel.CandidatesForJudges = remainingCandidates;

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

        [HttpPost]
        [Authorize(Roles = "Supervisor, Urp")]
        public async Task<IActionResult> AddJudges(AddJudgesRequestModel requestModel)
        {
            try
            {             
                var judgesIds = JsonConvert.DeserializeObject<List<string>>(requestModel.judgesIds);
                var assessmentId = requestModel.assessmentId;

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
                    await _assessmentService.AddJudgeForAssessment(Convert.ToInt32(judgeId), assessmentId);
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
        public async Task<IActionResult> DeleteJudge(DeleteJudgeRequestModel requestModel)
        {
            try
            {
                var deleteJudgeRes = await _assessmentService.DeleteJudgeForAssessment(requestModel.judgeId, requestModel.assessmentId);

                if (!deleteJudgeRes.IsSuccess)
                {
                    return BadRequest(new
                    {
                        error = "Произошла ошибка при удалении.",
                        details = deleteJudgeRes.Description,
                    });
                }

                return Ok("Удаление прошло успешно");
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    error = "Произошла ошибка при удалении.",
                    details = ex.Message
                });
            }
        }
    }
}