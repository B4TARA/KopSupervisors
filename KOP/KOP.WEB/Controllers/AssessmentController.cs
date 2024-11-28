using KOP.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StatusCodes = KOP.Common.Enums.StatusCodes;

namespace KOP.WEB.Controllers
{
    public class AssessmentController : Controller
    {
        private readonly IAssessmentService _assessmentService;
        private readonly IEmployeeService _employeeService;

        public AssessmentController(IAssessmentService assessmentService, IEmployeeService employeeService)
        {
            _assessmentService = assessmentService;
            _employeeService = employeeService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAssessmentTypes(int employeeId)
        {
            var response = await _assessmentService.GetAssessmentTypes(employeeId);

            if (response.StatusCode != StatusCodes.OK)
            {
                // Возвращаем сообщение об ошибке в формате JSON
                return Json(new { success = false, message = response.Description });
            }

            return Json(new { success = true, data = response.Data });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetLastAssessments(int employeeId)
        {
            var response = await _employeeService.GetEmployeeLastAssessments(employeeId, employeeId);

            if (response.StatusCode != StatusCodes.OK)
            {
                // Возвращаем сообщение об ошибке в формате JSON
                return Json(new { success = false, message = response.Description });
            }

            return Json(new { success = true, data = response.Data });
        }
    }
}