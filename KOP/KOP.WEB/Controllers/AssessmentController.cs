using KOP.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KOP.WEB.Controllers
{
    public class AssessmentController : Controller
    {
        private readonly IUserService _userService;

        public AssessmentController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetLastAssessments(int employeeId)
        {
            try 
            {
                var lastAssessmentsOfEachType = await _userService.GetUserLastAssessmentsOfEachAssessmentType(employeeId, employeeId);

                return Json(new { success = true, data = lastAssessmentsOfEachType });
            }

            catch
            {
                // LOG!!!
                return Json(new { success = false, message = "An unexpected error occurred. Please try again later." });
            }
        }
    }
}