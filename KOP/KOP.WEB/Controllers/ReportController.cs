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
                var supervisorId = Convert.ToInt32(User.FindFirstValue("Id"));
                var getSubordinateEmployeesRes = await _reportService.GetSubordinateUsersWithGrade(supervisorId);

                if (!getSubordinateEmployeesRes.HasData)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = getSubordinateEmployeesRes.StatusCode,
                        Message = getSubordinateEmployeesRes.Description
                    });
                }

                return View("SubordinateEmployees", getSubordinateEmployeesRes.Data);
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
                var getEmployeeGradesRes = await _reportService.GetEmployeeGrades(employeeId);

                if (!getEmployeeGradesRes.HasData)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = getEmployeeGradesRes.StatusCode,
                        Message = getEmployeeGradesRes.Description
                    });
                }

                return View("EmployeeGrades", getEmployeeGradesRes.Data);
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
                var generateGradeWordDocumentRes = await _reportService.GenerateGradeWordDocument(requestModel.gradeId);

                if (!generateGradeWordDocumentRes.HasData)
                {
                    return BadRequest(new
                    {
                        error = "Ошибка при создании документа Word.",
                        details = generateGradeWordDocumentRes.Description,
                    });
                }
                var fileName = $"Report_Grade_{requestModel.gradeId}.docx";
                return File(generateGradeWordDocumentRes.Data, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ошибка при создании документа Word.");
            }
        }

    }
}
