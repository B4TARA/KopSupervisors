using KOP.BLL.Interfaces;
using KOP.Common.Enums;
using KOP.WEB.Models.ViewModels;
using KOP.WEB.Models.ViewModels.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StatusCodes = KOP.Common.Enums.StatusCodes;

namespace KOP.WEB.Controllers
{
    public class GradeController : Controller
    {
        private readonly IGradeService _gradeService;
        private readonly IEmployeeService _employeeService;

        public GradeController(IGradeService gradeService, IEmployeeService employeeService)
        {
            _gradeService = gradeService;
            _employeeService = employeeService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetStrategicTasksPopUp(int gradeId)
        {
            try
            {
                var response = await _gradeService.GetGrade(gradeId, new List<GradeEntities> { GradeEntities.StrategicTasks });

                if (response.StatusCode != StatusCodes.OK || response.Data == null)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = response.StatusCode,
                        Message = response.Description,
                    });
                }

                var viewModel = new StrategicTasksViewModel
                {
                    GradeId = gradeId,
                    StrategicTasksConclusion = response.Data.StrategicTasksConclusion,
                    StrategicTasks = response.Data.StrategicTasks,
                };

                return View("_StrategicTasks", viewModel);
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
        [Authorize]
        public async Task<IActionResult> GetProjectsPopUp(int gradeId)
        {
            try
            {
                var response = await _gradeService.GetGrade(gradeId, new List<GradeEntities> { GradeEntities.Projects });

                if (response.StatusCode != StatusCodes.OK || response.Data == null)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = response.StatusCode,
                        Message = response.Description,
                    });
                }

                var viewModel = new ProjectsViewModel
                {
                    GradeId = gradeId,
                    Projects = response.Data.Projects,
                };

                return View("_Projects", viewModel);
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
        [Authorize]
        public async Task<IActionResult> GetKpisPopUp(int gradeId)
        {
            try
            {
                var response = await _gradeService.GetGrade(gradeId, new List<GradeEntities> { GradeEntities.Kpis });

                if (response.StatusCode != StatusCodes.OK || response.Data == null)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = response.StatusCode,
                        Message = response.Description,
                    });
                }

                var viewModel = new KpisViewModel
                {
                    GradeId = gradeId,
                    KpisConclusion = response.Data.KPIsConclusion,
                    Kpis = response.Data.Kpis,
                };

                return View("_Kpis", viewModel);
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
        [Authorize]
        public async Task<IActionResult> GetMarksPopUp(int gradeId)
        {
            try
            {
                var response = await _gradeService.GetGrade(gradeId, new List<GradeEntities> { GradeEntities.Marks });

                if (response.StatusCode != StatusCodes.OK || response.Data == null)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = response.StatusCode,
                        Message = response.Description,
                    });
                }

                var viewModel = new MarksViewModel
                {
                    GradeId = gradeId,
                    MarkTypes = response.Data.MarkTypes,
                };

                return View("_Marks", viewModel);
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
        [Authorize]
        public async Task<IActionResult> GetTrainingEventsPopUp(int gradeId)
        {
            try
            {
                var response = await _gradeService.GetGrade(gradeId, new List<GradeEntities> { GradeEntities.TrainingEvents });

                if (response.StatusCode != StatusCodes.OK || response.Data == null)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = response.StatusCode,
                        Message = response.Description,
                    });
                }

                var viewModel = new TrainingEventsViewModel
                {
                    GradeId = gradeId,
                    TrainingEvents = response.Data.TrainingEvents,
                };

                return View("_TrainingEvents", viewModel);
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
        [Authorize]
        public async Task<IActionResult> GetQualificationPopUp(int gradeId)
        {
            try
            {
                var response = await _gradeService.GetGrade(gradeId, new List<GradeEntities> { GradeEntities.Qualification });

                if (response.StatusCode != StatusCodes.OK || response.Data == null)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = response.StatusCode,
                        Message = response.Description,
                    });
                }

                var viewModel = new QualificationViewModel
                {
                    GradeId = gradeId,
                    QualificationConclusion = response.Data.QualificationConclusion,
                    Qualification = response.Data.Qualification,
                };

                return View("_Qualification", viewModel);
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
        [Authorize]
        public async Task<IActionResult> GetValueJudgmentPopUp(int gradeId)
        {
            try
            {
                var response = await _gradeService.GetGrade(gradeId, new List<GradeEntities> { GradeEntities.ValueJudgment });

                if (response.StatusCode != StatusCodes.OK || response.Data == null)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = response.StatusCode,
                        Message = response.Description,
                    });
                }

                var viewModel = new ValueJudgmentViewModel
                {
                    GradeId = gradeId,
                    ValueJudgment = response.Data.ValueJudgment,
                };

                return View("_ValueJudgment", viewModel);
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
        [Authorize]
        public async Task<IActionResult> EditValueJudgment(ValueJudgmentViewModel viewModel)
        {
            try
            {
                var editRes = await _gradeService.EditValueJudgment(viewModel.ValueJudgment);

                if (editRes.StatusCode != StatusCodes.OK)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = editRes.StatusCode,
                        Message = editRes.Description,
                    });
                }

                return RedirectToAction("Index", "Home");
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
        [Authorize]
        public async Task<IActionResult> GetCorporateCompetenciesPopUp(int employeeId, int gradeId)
        {
            try
            {
                var lastAssessmentRes = await _employeeService.GetLastAssessment(employeeId, 2);

                if (lastAssessmentRes.StatusCode != StatusCodes.OK || lastAssessmentRes.Data == null)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = lastAssessmentRes.StatusCode,
                        Message = lastAssessmentRes.Description,
                    });
                }

                var gradeRes = await _gradeService.GetGrade(gradeId, new List<GradeEntities>());

                if (gradeRes.StatusCode != StatusCodes.OK || gradeRes.Data == null)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = gradeRes.StatusCode,
                        Message = gradeRes.Description,
                    });
                }

                var viewModel = new CorporateCompetenciesViewModel
                {
                    EmployeeId = employeeId,
                    GradeId = gradeId,
                    CorporateCompetenciesConclusion = gradeRes.Data.CorporateCompetenciesConclusion,
                    LastAssessment = lastAssessmentRes.Data
                };

                return View("_CorporateCompetencies", viewModel);
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
        [Authorize]
        public async Task<IActionResult> GetManagmentCompetenciesPopUp(int employeeId, int gradeId)
        {
            try
            {
                var lastAssessmentRes = await _employeeService.GetLastAssessment(employeeId, 1);

                if (lastAssessmentRes.StatusCode != StatusCodes.OK || lastAssessmentRes.Data == null)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = lastAssessmentRes.StatusCode,
                        Message = lastAssessmentRes.Description,
                    });
                }

                var gradeRes = await _gradeService.GetGrade(gradeId, new List<GradeEntities>());

                if (gradeRes.StatusCode != StatusCodes.OK || gradeRes.Data == null)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = gradeRes.StatusCode,
                        Message = gradeRes.Description,
                    });
                }

                var viewModel = new ManagmentCompetenciesViewModel
                {
                    EmployeeId = employeeId,
                    GradeId = gradeId,
                    ManagmentCompetenciesConclusion = gradeRes.Data.ManagmentCompetenciesConclusion,
                    LastAssessment = lastAssessmentRes.Data
                };

                return View("_ManagmentCompetencies", viewModel);
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