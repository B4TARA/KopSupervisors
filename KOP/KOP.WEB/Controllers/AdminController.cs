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

                return View("Users", allUsersSummariesDtos.OrderBy(x => x.FullName).ToList());
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
        public async Task<IActionResult> GetUserLayout(int userId)
        {
            if (userId <= 0)
            {
                _logger.LogWarning("Invalid userId: {userId}", userId);
                return BadRequest("Invalid user ID.");
            }

            try
            {
                var user = await _unitOfWork.Users.GetAsync(x => x.Id == userId);

                if (user == null)
                {
                    _logger.LogWarning($"User with ID {userId} not found.");
                    return BadRequest($"User with ID {userId} not found.");
                }

                var viewModel = new UserLayoutViewModel
                {
                    UserId = user.Id,
                    FullName = user.FullName,
                };

                return PartialView("_UserLayoutPartial", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AdminController.GetUserLayout] : ");

                return View("Error", new ErrorViewModel
                {
                    StatusCode = StatusCodes.InternalServerError,
                    Message = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Urp")]
        public async Task<IActionResult> GetUserSubordinates(int userId)
        {
            if (userId <= 0)
            {
                _logger.LogWarning("Invalid userId: {userId}", userId);
                return BadRequest("Invalid user ID.");
            }

            try
            {
                var user = await _unitOfWork.Users.GetAsync(x => x.Id == userId, includeProperties: "SubordinateSubdivisions");

                if (user == null)
                {
                    _logger.LogWarning($"User with ID {userId} not found.");
                    return BadRequest($"User with ID {userId} not found.");
                }

                var allsubdivisions = await _unitOfWork.Subdivisions.GetAllAsync();
                var userSubdivisions = user.SubordinateSubdivisions;

                var viewModel = new UserSubordinatesViewModel
                {
                    UserId = user.Id,
                };

                foreach (var subdivision in allsubdivisions)
                {
                    viewModel.Subdivisions.Add(new SubdivisionSummaryDto
                    {
                        Id = subdivision.Id,
                        Name = subdivision.Name,
                        ParentId = subdivision.ParentId,
                        IsSelected = userSubdivisions.Any(x => x.Id == subdivision.Id)
                    });
                }

                return PartialView("_UserSubordinatesPartial", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AdminController.GetUserSubordinates] : ");

                return View("Error", new ErrorViewModel
                {
                    StatusCode = StatusCodes.InternalServerError,
                    Message = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Urp")]
        public async Task<IActionResult> GetUserSystemRoles(int userId)
        {
            if (userId <= 0)
            {
                _logger.LogWarning("Invalid userId: {userId}", userId);
                return BadRequest("Invalid user ID.");
            }

            try
            {
                var user = await _unitOfWork.Users.GetAsync(x => x.Id == userId);

                if (user == null)
                {
                    _logger.LogWarning($"User with ID {userId} not found.");
                    return BadRequest($"User with ID {userId} not found.");
                }

                var viewModel = new UserSystemRolesViewModel
                {
                    Id = user.Id,
                    FullName = user.FullName,
                };

                foreach (var systemRole in Enum.GetValues(typeof(SystemRoles)))
                {
                    viewModel.CheckboxRoles.Add(new CheckboxRole
                    {
                        Role = (SystemRoles)systemRole,
                        Checked = user.SystemRoles.Contains((SystemRoles)systemRole),
                    });
                }

                return PartialView("_UserSystemRolesPartial", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AdminController.GetUserSystemRoles] : ");

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

        [HttpPost]
        [Authorize(Roles = "Urp")]
        public async Task<IActionResult> UpdateUserSystemRoles(UserSystemRolesViewModel viewModel)
        {
            if (viewModel.Id <= 0)
            {
                _logger.LogWarning("Invalid userId: {userId}", viewModel.Id);
                return BadRequest("Invalid user ID.");
            }

            try
            {
                var user = await _unitOfWork.Users.GetAsync(x => x.Id == viewModel.Id);

                if (user == null)
                {
                    _logger.LogWarning($"User with ID {viewModel.Id} not found.");
                    return BadRequest($"User with ID {viewModel.Id} not found.");
                }

                user.SystemRoles.Clear();
                foreach (var checkboxRole in viewModel.CheckboxRoles)
                {
                    if (!checkboxRole.Checked)
                    {
                        continue;
                    }
                    user.SystemRoles.Add(checkboxRole.Role);
                }

                _unitOfWork.Users.Update(user);
                await _unitOfWork.CommitAsync();

                return Ok("Сохранение прошло успешно");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AdminController.UpdateUserSystemRoles] : ");
                return BadRequest(new
                {
                    error = "Произошла ошибка при сохранении.",
                    details = ex.Message
                });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Urp")]
        public async Task<IActionResult> UpdateUserSubordinates([FromBody] UserSubordinatesViewModel viewModel)
        {
            if (viewModel.UserId <= 0)
            {
                _logger.LogWarning("Invalid userId: {userId}", viewModel.UserId);
                return BadRequest("Invalid user ID.");
            }

            try
            {
                var user = await _unitOfWork.Users.GetAsync(x => x.Id == viewModel.UserId, includeProperties: "SubordinateSubdivisions");

                if (user == null)
                {
                    _logger.LogWarning($"User with ID {viewModel.UserId} not found.");
                    return BadRequest($"User with ID {viewModel.UserId} not found.");
                }

                user.SubordinateSubdivisions.Clear();
                foreach (var id in viewModel.SubordinateSubdivisionsIds)
                {
                    var dbSubdivision = await _unitOfWork.Subdivisions.GetAsync(x => x.Id == id);
                    if (dbSubdivision == null)
                    {
                        _logger.LogWarning($"Subdivision with ID {id} not found.");
                        continue;
                    }

                    // Работает только с одним уровнем вложенности
                    // Проверяем, есть ли ParentId в списке SubordinateSubdivisionsIds
                    if (dbSubdivision.ParentId.HasValue && viewModel.SubordinateSubdivisionsIds.Contains(dbSubdivision.ParentId.Value))
                    {
                        continue; // Пропускаем добавление
                    }

                    user.SubordinateSubdivisions.Add(dbSubdivision);
                }

                _unitOfWork.Users.Update(user);
                await _unitOfWork.CommitAsync();

                return Ok("Сохранение прошло успешно");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AdminController.UpdateUserSubordinates] : ");
                return BadRequest(new
                {
                    error = "Произошла ошибка при сохранении.",
                    details = ex.Message
                });
            }
        }
    }
}