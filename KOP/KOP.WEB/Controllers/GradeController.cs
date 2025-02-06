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
        private readonly IUserService _userService;

        public GradeController(IGradeService gradeService, IUserService userService)
        {
            _gradeService = gradeService;
            _userService = userService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetStrategicTasksPopup(int gradeId)
        {
            try
            {
                var gradeRes = await _gradeService.GetGrade(gradeId, new List<GradeEntities> { GradeEntities.StrategicTasks });

                if (!gradeRes.HasData)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = gradeRes.StatusCode,
                        Message = gradeRes.Description,
                    });
                }

                var viewModel = new StrategicTasksViewModel
                {
                    GradeId = gradeId,
                    Conclusion = gradeRes.Data.StrategicTasksConclusion,
                    StrategicTasks = gradeRes.Data.StrategicTasks,
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
        public async Task<IActionResult> GetProjectsPopup(int gradeId)
        {
            try
            {
                var gradeRes = await _gradeService.GetGrade(gradeId, new List<GradeEntities> { GradeEntities.Projects });

                if (!gradeRes.HasData)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = gradeRes.StatusCode,
                        Message = gradeRes.Description,
                    });
                }

                var viewModel = new ProjectsViewModel
                {
                    GradeId = gradeId,
                    Projects = gradeRes.Data.Projects,
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
        public async Task<IActionResult> GetKpisPopup(int gradeId)
        {
            try
            {
                var gradeRes = await _gradeService.GetGrade(gradeId, new List<GradeEntities> { GradeEntities.Kpis });

                if (!gradeRes.HasData)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = gradeRes.StatusCode,
                        Message = gradeRes.Description,
                    });
                }

                var viewModel = new KpisViewModel
                {
                    GradeId = gradeId,
                    Conclusion = gradeRes.Data.KPIsConclusion,
                    Kpis = gradeRes.Data.Kpis,
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
        public async Task<IActionResult> GetMarksPopup(int gradeId)
        {
            try
            {
                var gradeRes = await _gradeService.GetGrade(gradeId, new List<GradeEntities> { GradeEntities.Marks });

                if (!gradeRes.HasData)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = gradeRes.StatusCode,
                        Message = gradeRes.Description,
                    });
                }

                var viewModel = new MarksViewModel
                {
                    GradeId = gradeId,
                    MarkTypes = gradeRes.Data.MarkTypes,
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
        public async Task<IActionResult> GetTrainingEventsPopup(int gradeId)
        {
            try
            {
                var gradeRes = await _gradeService.GetGrade(gradeId, new List<GradeEntities> { GradeEntities.TrainingEvents });

                if (!gradeRes.HasData)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = gradeRes.StatusCode,
                        Message = gradeRes.Description,
                    });
                }

                var viewModel = new TrainingEventsViewModel
                {
                    GradeId = gradeId,
                    TrainingEvents = gradeRes.Data.TrainingEvents,
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
        public async Task<IActionResult> GetQualificationPopup(int gradeId)
        {
            try
            {
                var gradeRes = await _gradeService.GetGrade(gradeId, new List<GradeEntities> { GradeEntities.Qualification });

                if (!gradeRes.HasData)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = gradeRes.StatusCode,
                        Message = gradeRes.Description,
                    });
                }

                var viewModel = new QualificationViewModel
                {
                    GradeId = gradeId,
                    Conclusion = gradeRes.Data.QualificationConclusion,
                    Qualification = gradeRes.Data.Qualification,
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
        public async Task<IActionResult> GetValueJudgmentPopup(int gradeId)
        {
            try
            {
                var gradeRes = await _gradeService.GetGrade(gradeId, new List<GradeEntities> { GradeEntities.ValueJudgment });

                if (!gradeRes.HasData)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = gradeRes.StatusCode,
                        Message = gradeRes.Description,
                    });
                }

                var viewModel = new ValueJudgmentViewModel
                {
                    GradeId = gradeId,
                    ValueJudgment = gradeRes.Data.ValueJudgment,
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

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetCorporateCompetenciesPopup(int employeeId, int gradeId)
        {
            try
            {
                // 2 - это id assessmentType ( являющегося корпоративными компетенциями)
                // поменять на Enum + новая строка в assessmentType
                var lastAssessmentRes = await _userService.GetLastAssessmentByAssessmentType(employeeId, 2);

                if (!lastAssessmentRes.HasData)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = lastAssessmentRes.StatusCode,
                        Message = lastAssessmentRes.Description,
                    });
                }

                var gradeRes = await _gradeService.GetGrade(gradeId, new List<GradeEntities>());

                if (!gradeRes.HasData)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = gradeRes.StatusCode,
                        Message = gradeRes.Description,
                    });
                }

                var viewModel = new CorporateCompetenciesViewModel
                {
                    Conclusion = gradeRes.Data.CorporateCompetenciesConclusion,
                    LastCompletedAssessmentResults = lastAssessmentRes.Data.AssessmentResults.Where(x => x.SystemStatus == SystemStatuses.COMPLETED).ToList(),
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
        public async Task<IActionResult> GetManagmentCompetenciesPopup(int employeeId, int gradeId)
        {
            try
            {
                // 1 - это id assessmentType ( являющегося управленческими компетенциями)
                // поменять на Enum + новая строка в assessmentType
                var lastAssessmentRes = await _userService.GetLastAssessmentByAssessmentType(employeeId, 1);

                if (!lastAssessmentRes.HasData)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = lastAssessmentRes.StatusCode,
                        Message = lastAssessmentRes.Description,
                    });
                }

                var gradeRes = await _gradeService.GetGrade(gradeId, new List<GradeEntities>());

                if (!gradeRes.HasData)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = gradeRes.StatusCode,
                        Message = gradeRes.Description,
                    });
                }

                var viewModel = new ManagmentCompetenciesViewModel
                {
                    Conclusion = gradeRes.Data.ManagmentCompetenciesConclusion,
                    LastCompletedAssessmentResults = lastAssessmentRes.Data.AssessmentResults.Where(x => x.SystemStatus == SystemStatuses.COMPLETED).ToList()
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



        // POST methods

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditStrategicTasks(StrategicTasksViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("_StrategicTasks", viewModel);
                }

                var getGradeRes = await _gradeService.GetGrade(viewModel.GradeId, new List<GradeEntities> { GradeEntities.StrategicTasks });

                if (!getGradeRes.HasData)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = getGradeRes.StatusCode,
                        Message = getGradeRes.Description,
                    });
                }

                getGradeRes.Data.StrategicTasks = viewModel.StrategicTasks;
                getGradeRes.Data.StrategicTasksConclusion = viewModel.Conclusion;

                var editGradeRes = await _gradeService.EditGrade(getGradeRes.Data);

                if (!editGradeRes.HasData)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = getGradeRes.StatusCode,
                        Message = getGradeRes.Description,
                    });
                }

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

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditKpis(KpisViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("_Kpis", viewModel);
                }

                var getGradeRes = await _gradeService.GetGrade(viewModel.GradeId, new List<GradeEntities> { GradeEntities.Kpis });

                if (!getGradeRes.HasData)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = getGradeRes.StatusCode,
                        Message = getGradeRes.Description,
                    });
                }

                getGradeRes.Data.Kpis = viewModel.Kpis;
                getGradeRes.Data.KPIsConclusion = viewModel.Conclusion;

                var editGradeRes = await _gradeService.EditGrade(getGradeRes.Data);

                if (!editGradeRes.HasData)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = getGradeRes.StatusCode,
                        Message = getGradeRes.Description,
                    });
                }

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

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditProjects(ProjectsViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("_Projects", viewModel);
                }

                var getGradeRes = await _gradeService.GetGrade(viewModel.GradeId, new List<GradeEntities> { GradeEntities.Projects });

                if (!getGradeRes.HasData)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = getGradeRes.StatusCode,
                        Message = getGradeRes.Description,
                    });
                }

                getGradeRes.Data.Projects = viewModel.Projects;

                var editGradeRes = await _gradeService.EditGrade(getGradeRes.Data);

                if (!editGradeRes.HasData)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = getGradeRes.StatusCode,
                        Message = getGradeRes.Description,
                    });
                }

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

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditMarks(MarksViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("_Marks", viewModel);
                }

                var getGradeRes = await _gradeService.GetGrade(viewModel.GradeId, new List<GradeEntities> { GradeEntities.Marks });

                if (!getGradeRes.HasData)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = getGradeRes.StatusCode,
                        Message = getGradeRes.Description,
                    });
                }

                getGradeRes.Data.MarkTypes = viewModel.MarkTypes;

                var editGradeRes = await _gradeService.EditGrade(getGradeRes.Data);

                if (!editGradeRes.HasData)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = getGradeRes.StatusCode,
                        Message = getGradeRes.Description,
                    });
                }

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

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditValueJudgment(ValueJudgmentViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("_ValueJudgment", viewModel);
                }

                var getGradeRes = await _gradeService.GetGrade(viewModel.GradeId, new List<GradeEntities> { GradeEntities.ValueJudgment });

                if (!getGradeRes.HasData)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = getGradeRes.StatusCode,
                        Message = getGradeRes.Description,
                    });
                }

                getGradeRes.Data.ValueJudgment = viewModel.ValueJudgment;

                var editGradeRes = await _gradeService.EditGrade(getGradeRes.Data);

                if (!editGradeRes.HasData)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = getGradeRes.StatusCode,
                        Message = getGradeRes.Description,
                    });
                }

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
        public async Task<IActionResult> EditQualification(QualificationViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("_Qualification", viewModel);
                }

                var getGradeRes = await _gradeService.GetGrade(viewModel.GradeId, new List<GradeEntities> { GradeEntities.Qualification });

                if (!getGradeRes.HasData)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = getGradeRes.StatusCode,
                        Message = getGradeRes.Description,
                    });
                }

                getGradeRes.Data.Qualification = viewModel.Qualification;
                getGradeRes.Data.QualificationConclusion = viewModel.Conclusion;

                var editGradeRes = await _gradeService.EditGrade(getGradeRes.Data);

                if (!editGradeRes.HasData)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = getGradeRes.StatusCode,
                        Message = getGradeRes.Description,
                    });
                }

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
    }
}