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
                var gradeRes = await _gradeService.GetGrade(gradeId, new List<GradeEntities> { GradeEntities.Qualification });

                if (!gradeRes.HasData)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = gradeRes.StatusCode,
                        Message = gradeRes.Description,
                    });
                }

                var conclusionEditAccess = User.IsInRole("Urp");
                var editAccess = User.IsInRole("Urp");
                var viewAccess = gradeRes.Data.IsQualificationFinalized || editAccess;

                var viewModel = new QualificationViewModel
                {
                    GradeId = gradeId,
                    Conclusion = gradeRes.Data.QualificationConclusion,
                    Qualification = gradeRes.Data.Qualification,
                    EditAccess = editAccess,
                    ViewAccess = viewAccess,
                    ConclusionEditAccess = conclusionEditAccess,
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

                var getGradeRes = await _gradeService.GetGrade(viewModel.GradeId, new List<GradeEntities> { GradeEntities.Qualification });

                if (!getGradeRes.HasData)
                {
                    return BadRequest(new
                    {
                        error = "Произошла ошибка при сохранении.",
                        details = getGradeRes.Description,
                    });
                }

                getGradeRes.Data.Qualification = viewModel.Qualification;
                getGradeRes.Data.QualificationConclusion = viewModel.Conclusion;
                getGradeRes.Data.IsQualificationFinalized = viewModel.IsFinalized;

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
        public async Task<IActionResult> DeletePreviousJob(int id)
        {
            try
            {
                var deletePreviousJobRes = await _gradeService.DeletePreviousJob(id);

                if (!deletePreviousJobRes.IsSuccess)
                {
                    return BadRequest(new
                    {
                        error = "Произошла ошибка при удалении.",
                        details = deletePreviousJobRes.Description,
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