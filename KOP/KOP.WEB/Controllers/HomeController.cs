using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KOP.WEB.Controllers
{
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public IActionResult Index()
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            else if (User.IsInRole("Employee"))
            {
                return RedirectToAction("GetEmployeeLayout", "Employee");
            }
            else
            {
                return RedirectToAction("GetSupervisorLayout", "Supervisor");
            }
        }
    }
}