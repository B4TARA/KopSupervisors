using KOP.BLL.Interfaces;
using KOP.Common.Enums;
using KOP.WEB.Models.ViewModels.Shared;
using KOP.WEB.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StatusCodes = KOP.Common.Enums.StatusCodes;

namespace KOP.WEB.Controllers
{
    public class KpiController : Controller
    {
        private readonly IGradeService _gradeService;

        public KpiController(IGradeService gradeService)
        {
            _gradeService = gradeService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetPopup(int gradeId)
        {
            try
            {
                var gradeRes = await _gradeService.GetGradeDto(gradeId, new List<GradeEntities> { GradeEntities.Kpis });

                if (!gradeRes.HasData)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = gradeRes.StatusCode,
                        Message = gradeRes.Description,
                    });
                }

                var conclusionEditAccess = User.IsInRole("Urp");
                var editAccess = (User.IsInRole("Umst") && !gradeRes.Data.IsKpisFinalized) || User.IsInRole("Urp");
                var viewAccess = gradeRes.Data.IsKpisFinalized || editAccess;

                var viewModel = new KpisViewModel
                {
                    GradeId = gradeId,
                    Conclusion = gradeRes.Data.KPIsConclusion,
                    Kpis = gradeRes.Data.Kpis,
                    EditAccess = editAccess,
                    ViewAccess = viewAccess,
                    ConclusionEditAccess = conclusionEditAccess,
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

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditAll(KpisViewModel viewModel)
        {
            try
            {
                var getGradeRes = await _gradeService.GetGradeDto(viewModel.GradeId, new List<GradeEntities> { GradeEntities.Kpis });

                if (!getGradeRes.HasData)
                {
                    return BadRequest(new
                    {
                        error = "Произошла ошибка при сохранении.",
                        details = getGradeRes.Description,
                    });
                }

                getGradeRes.Data.Kpis = viewModel.Kpis;
                getGradeRes.Data.KPIsConclusion = viewModel.Conclusion;
                getGradeRes.Data.IsKpisFinalized = viewModel.IsFinalized;

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

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleteKpiRes = await _gradeService.DeleteKpi(id);

                if (!deleteKpiRes.IsSuccess)
                {
                    return BadRequest(new
                    {
                        error = "Произошла ошибка при удалении.",
                        details = deleteKpiRes.Description,
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
