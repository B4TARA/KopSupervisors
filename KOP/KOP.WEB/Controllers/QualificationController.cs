using KOP.BLL.Interfaces;
using KOP.Common.Enums;
using KOP.WEB.Models.ViewModels.Shared;
using KOP.WEB.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StatusCodes = KOP.Common.Enums.StatusCodes;

namespace KOP.WEB.Controllers
{
    public class QualificationController : Controller
    {
        private readonly IGradeService _gradeService;

        public QualificationController(IGradeService gradeService)
        {
            _gradeService = gradeService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetPopup(int gradeId)
        {
            try
            {
                var gradeDto = await _gradeService.GetGradeDto(gradeId, new List<GradeEntities> { GradeEntities.Qualification });

                var conclusionEditAccess = User.IsInRole("Urp");
                var editAccess = User.IsInRole("Urp");
                var viewAccess = gradeDto.IsQualificationFinalized || editAccess;

                var viewModel = new QualificationViewModel
                {
                    GradeId = gradeId,
                    SelectedUserFullName = HttpContext.Session.GetString("SelectedUserFullName") ?? "-",
                    Conclusion = gradeDto.QualificationConclusion,
                    Qualification = gradeDto.QualificationDto,
                    EditAccess = editAccess,
                    ViewAccess = viewAccess,
                    ConclusionEditAccess = conclusionEditAccess,
                };

                return View("_QualificationPartial", viewModel);
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
        public async Task<IActionResult> Edit(QualificationViewModel viewModel)
        {
            try
            {
                var editAccess = User.IsInRole("Urp");

                if (!editAccess)
                {
                    return BadRequest(new
                    {
                        error = "Произошла ошибка при сохранении.",
                        details = "Доступ запрещен",
                    });
                }

                var gradeDto = await _gradeService.GetGradeDto(viewModel.GradeId, new List<GradeEntities> { GradeEntities.Qualification });

                gradeDto.QualificationDto = viewModel.Qualification;
                gradeDto.QualificationConclusion = viewModel.Conclusion;
                gradeDto.IsQualificationFinalized = viewModel.IsFinalized;

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
        public async Task<IActionResult> DeletePreviousJob(int id)
        {
            try
            {
                await _gradeService.DeletePreviousJob(id);

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

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteHigherEducation(int id)
        {
            try
            {
                await _gradeService.DeleteHigherEducation(id);

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