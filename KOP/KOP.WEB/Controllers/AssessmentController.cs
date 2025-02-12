using KOP.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StatusCodes = KOP.Common.Enums.StatusCodes;

namespace KOP.WEB.Controllers
{
    public class AssessmentController : Controller
    {
        private readonly IAssessmentService _assessmentService;
        private readonly IUserService _userService;

        public AssessmentController(IAssessmentService assessmentService, IUserService userService)
        {
            _assessmentService = assessmentService;
            _userService = userService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAssessmentTypes(int employeeId)
        {
            var response = await _userService.GetAssessmentTypesForUser(employeeId);

            if (response.StatusCode != StatusCodes.OK)
            {
                return Json(new { success = false, message = response.Description });
            }

            return Json(new { success = true, data = response.Data });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetLastAssessments(int employeeId)
        {
            var response = await _userService.GetUserLastAssessmentsOfEachAssessmentType(employeeId, employeeId);

            if (response.StatusCode != StatusCodes.OK)
            {
                return Json(new { success = false, message = response.Description });
            }

            return Json(new { success = true, data = response.Data });
        }
    }
}