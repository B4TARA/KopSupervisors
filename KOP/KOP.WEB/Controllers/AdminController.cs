using KOP.Common.Dtos;
using KOP.Common.Dtos.AssessmentDtos;
using KOP.Common.Enums;
using KOP.DAL.Interfaces;
using KOP.WEB.Models.ViewModels;
using KOP.WEB.Models.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StatusCodes = KOP.Common.Enums.StatusCodes;

namespace KOP.WEB.Controllers
{
    public class AdminController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IUnitOfWork unitOfWork, ILogger<AdminController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Urp")]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var allUsers = await _unitOfWork.Users.GetAllAsync();
                var allUsersSummariesDtos = allUsers.Select(user => new UserSummaryDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Position = user.Position,
                    SubdivisionFromFile = user.SubdivisionFromFile,
                }).ToList();

                return View("Users", allUsersSummariesDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AdminController.GetUsers] : ");

                return View("Error", new ErrorViewModel
                {
                    StatusCode = StatusCodes.InternalServerError,
                    Message = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Urp")]
        public async Task<IActionResult> GetCorporateMatrix()
        {
            try
            {
                var allCorporateMatrixElements = await _unitOfWork.AssessmentMatrixElements
                    .GetAllAsync(x => x.Matrix.SystemType == SystemAssessmentTypes.СorporateСompetencies);
                var allCorporateMatrixElementsDtos = allCorporateMatrixElements.Select(element => new AssessmentMatrixElementDto
                {
                    Id = element.Id,
                    Row = element.Row,
                    Column = element.Column,
                    Value = element.Value,
                    HtmlClassName = element.HtmlClassName,
                }).ToList();
                var viewModel = new AssessmentMatrixViewModel
                {
                    Elements = allCorporateMatrixElementsDtos,
                    SystemAssessmentType = SystemAssessmentTypes.СorporateСompetencies,
                };

                return View("AssessmentMatrix", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AdminController.GetCorporateMatrix] : ");

                return View("Error", new ErrorViewModel
                {
                    StatusCode = StatusCodes.InternalServerError,
                    Message = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Urp")]
        public IActionResult GetSubdivisionsTree()
        {
            try
            {              
                return View("SubdivisionsTree");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AdminController.GetSubdivisionsTree] : ");

                return View("Error", new ErrorViewModel
                {
                    StatusCode = StatusCodes.InternalServerError,
                    Message = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Urp")]
        public async Task<IActionResult> GetManagmentMatrix()
        {
            try
            {
                var allManagmentMatrixElements = await _unitOfWork.AssessmentMatrixElements
                    .GetAllAsync(x => x.Matrix.SystemType == SystemAssessmentTypes.ManagementCompetencies);
                var allManagmentMatrixElementsDtos = allManagmentMatrixElements.Select(element => new AssessmentMatrixElementDto
                {
                    Id = element.Id,
                    Row = element.Row,
                    Column = element.Column,
                    Value = element.Value,
                    HtmlClassName = element.HtmlClassName,
                }).ToList();
                var viewModel = new AssessmentMatrixViewModel
                {
                    Elements = allManagmentMatrixElementsDtos,
                    SystemAssessmentType = SystemAssessmentTypes.ManagementCompetencies,
                };

                return View("AssessmentMatrix", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AdminController.GetManagmentMatrix] : ");

                return View("Error", new ErrorViewModel
                {
                    StatusCode = StatusCodes.InternalServerError,
                    Message = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Urp")]
        public async Task<IActionResult> UpdateCorporateMatrix(AssessmentMatrixViewModel viewModel)
        {
            try
            {
                foreach (var element in viewModel.Elements)
                {
                    var existingElement = await _unitOfWork.AssessmentMatrixElements.GetAsync(x => x.Id == element.Id);
                    if (existingElement != null)
                    {
                        existingElement.Value = element.Value;
                    }
                }

                await _unitOfWork.CommitAsync();
                return Ok("Сохранение прошло успешно");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AdminController.UpdateCorporateMatrix] : ");
                return BadRequest(new
                {
                    error = "Произошла ошибка при сохранении.",
                    details = ex.Message
                });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Urp")]
        public async Task<IActionResult> UpdateMatrix(AssessmentMatrixViewModel viewModel)
        {
            try
            {
                foreach (var element in viewModel.Elements)
                {
                    var existingElement = await _unitOfWork.AssessmentMatrixElements.GetAsync(x => x.Id == element.Id);
                    if (existingElement != null)
                    {
                        existingElement.Value = element.Value;
                    }
                }

                await _unitOfWork.CommitAsync();
                return Ok("Сохранение прошло успешно");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AdminController.UpdateMatrix] : ");
                return BadRequest(new
                {
                    error = "Произошла ошибка при сохранении.",
                    details = ex.Message
                });
            }
        }

    }
}