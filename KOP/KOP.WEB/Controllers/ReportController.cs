using System.Security.Claims;
using DocumentFormat.OpenXml.Office2016.Excel;
using KOP.BLL.Interfaces;
using KOP.WEB.Models.RequestModels;
using KOP.WEB.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPOI.XWPF.UserModel;
using Org.BouncyCastle.Asn1.Ocsp;
using StatusCodes = KOP.Common.Enums.StatusCodes;

namespace KOP.WEB.Controllers
{
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetSubordinateEmployees()
        {
            try
            {
                var currentUserId = Convert.ToInt32(User.FindFirstValue("Id"));
                var subordinateEmployees = await _reportService.GetSubordinateUsersWithGrade(currentUserId);

                return View("SubordinateEmployees", subordinateEmployees);
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
        public async Task<IActionResult> GetEmployeeGrades(int employeeId)
        {
            try
            {
                var employeeGradeDto = await _reportService.GetEmployeeGrades(employeeId);

                return View("EmployeeGrades", employeeGradeDto);
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
        public async Task<IActionResult> GenerateGradeWordDocument([FromBody] GenerateGradeWordDocumentRequestModel requestModel)
        {
            try
            {
                var document = await _reportService.GenerateGradeWordDocument(requestModel.gradeId);
                var fileName = $"Report_Grade_{requestModel.gradeId}.docx";

                return File(document, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ошибка при создании документа Word.");
            }
        }

    }
}
