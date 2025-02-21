using KOP.BLL.Interfaces;
using KOP.Common.Enums;
using KOP.WEB.Models.ViewModels.Shared;
using KOP.WEB.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StatusCodes = KOP.Common.Enums.StatusCodes;

namespace KOP.WEB.Controllers
{
    public class ValueJudgmentController : Controller
    {
        private readonly IGradeService _gradeService;

        public ValueJudgmentController(IGradeService gradeService)
        {
            _gradeService = gradeService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetPopup(int gradeId)
        {
            try
            {
                var gradeRes = await _gradeService.GetGradeDto(gradeId, new List<GradeEntities> { GradeEntities.ValueJudgment });

                if (!gradeRes.HasData)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = gradeRes.StatusCode,
                        Message = gradeRes.Description,
                    });
                }

                var editAccess = (User.IsInRole("Supervisor") && !gradeRes.Data.IsValueJudgmentFinalized) || User.IsInRole("Urp");
                var viewAccess = gradeRes.Data.IsValueJudgmentFinalized || editAccess;

                var viewModel = new ValueJudgmentViewModel
                {
                    GradeId = gradeId,
                    ValueJudgment = gradeRes.Data.ValueJudgment,
                    EditAccess = editAccess,
                    ViewAccess = viewAccess,
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
        public async Task<IActionResult> Edit(ValueJudgmentViewModel viewModel)
        {
            try
            {
                var getGradeRes = await _gradeService.GetGradeDto(viewModel.GradeId, new List<GradeEntities> { GradeEntities.ValueJudgment });

                if (!getGradeRes.HasData)
                {
                    return BadRequest(new
                    {
                        error = "Произошла ошибка при сохранении.",
                        details = getGradeRes.Description,
                    });
                }

                getGradeRes.Data.ValueJudgment = viewModel.ValueJudgment;
                getGradeRes.Data.IsValueJudgmentFinalized = viewModel.IsFinalized;

                var editGradeRes = await _gradeService.EditGrade(getGradeRes.Data);

                if (!editGradeRes.IsSuccess)
                {
                    return BadRequest(new
                    {
                        error = "Произошла ошибка при сохранении.",
                        details = getGradeRes.Description,
                    });
                }

                return Ok(viewModel.IsFinalized ? "Окончательное сохранение прошло успешно" : "Сохранение черновика прошло успешно");
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
    }
}