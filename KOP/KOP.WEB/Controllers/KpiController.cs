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
                var gradeDto = await _gradeService.GetGradeDto(gradeId, new List<GradeEntities> { GradeEntities.Kpis });
                var conclusionEditAccess = User.IsInRole("Urp");
                var editAccess = (User.IsInRole("Umst") && !gradeDto.IsKpisFinalized) || User.IsInRole("Urp");
                var viewAccess = gradeDto.IsKpisFinalized || editAccess;

                var viewModel = new KpisViewModel
                {
                    GradeId = gradeId,
                    // CHTCK THIS !!!
                    SelectedUserId = HttpContext.Session.GetInt32("SelectedUserId") ?? 0,
                    Conclusion = gradeDto.KPIsConclusion,
                    Kpis = gradeDto.KpiDtoList,
                    EditAccess = editAccess,
                    ViewAccess = viewAccess,
                    ConclusionEditAccess = conclusionEditAccess,
                };

                return View("_KpisPartial", viewModel);
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
                var gradeDto = await _gradeService.GetGradeDto(viewModel.GradeId, new List<GradeEntities> { GradeEntities.Kpis });

                gradeDto.KpiDtoList = viewModel.Kpis;
                gradeDto.KPIsConclusion = viewModel.Conclusion;
                gradeDto.IsKpisFinalized = viewModel.IsFinalized;

                await _gradeService.EditGrade(gradeDto);

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
                await _gradeService.DeleteKpi(id);

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
