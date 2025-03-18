using KOP.BLL.Interfaces;
using KOP.Common.Enums;
using KOP.WEB.Models.ViewModels.Shared;
using KOP.WEB.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StatusCodes = KOP.Common.Enums.StatusCodes;

namespace KOP.WEB.Controllers
{
    public class MarkController : Controller
    {
        private readonly IGradeService _gradeService;

        public MarkController(IGradeService gradeService)
        {
            _gradeService = gradeService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetPopup(int gradeId)
        {
            try
            {
                var gradeDto = await _gradeService.GetGradeDto(gradeId, new List<GradeEntities> { GradeEntities.Marks });

                var editAccess = User.IsInRole("Urp");
                var viewAccess = gradeDto.IsMarksFinalized || editAccess;

                var viewModel = new MarksViewModel
                {
                    GradeId = gradeId,
                    MarkTypes = gradeDto.MarkTypes,
                    EditAccess = editAccess,
                    ViewAccess = viewAccess,
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

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditAll(MarksViewModel viewModel)
        {
            try
            {
                var gradeDto = await _gradeService.GetGradeDto(viewModel.GradeId, new List<GradeEntities> { GradeEntities.Marks });

                gradeDto.MarkTypes = viewModel.MarkTypes;
                gradeDto.IsMarksFinalized = viewModel.IsFinalized;

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
                await _gradeService.DeleteMark(id);

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