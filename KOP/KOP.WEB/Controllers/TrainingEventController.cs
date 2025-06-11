using KOP.Common.Enums;
using KOP.WEB.Models.ViewModels.Shared;
using KOP.WEB.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using StatusCodes = KOP.Common.Enums.StatusCodes;
using Microsoft.AspNetCore.Mvc;
using KOP.BLL.Interfaces;

namespace KOP.WEB.Controllers
{
    public class TrainingEventController : Controller
    {
        private readonly IGradeService _gradeService;
        private readonly ILogger<TrainingEventController> _logger;

        public TrainingEventController(IGradeService gradeService, ILogger<TrainingEventController> logger)
        {
            _gradeService = gradeService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Supervisor, Urp, Employee, Curator, Uop")]
        public async Task<IActionResult> GetTrainingEventsPopup(int gradeId)
        {
            if (gradeId <= 0)
            {
                _logger.LogWarning("Invalid gradeId: {gradeId}", gradeId);
                return BadRequest("Invalid grade ID.");
            }

            try
            {
                var gradeDto = await _gradeService.GetGradeDto(gradeId, new List<GradeEntities> { GradeEntities.TrainingEvents });

                var viewModel = new TrainingEventsViewModel
                {
                    GradeId = gradeId,
                    TrainingEvents = gradeDto.TrainingEventDtoList,
                };

                return View("_TrainingEventsPartial", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[TrainingEventController.GetPopup] : ");
                return View("Error", new ErrorViewModel
                {
                    StatusCode = StatusCodes.InternalServerError,
                    Message = "An unexpected error occurred. Please try again later."
                });
            }
        }
    }
}