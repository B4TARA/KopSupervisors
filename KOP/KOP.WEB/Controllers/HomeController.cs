using KOP.WEB.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StatusCodes = KOP.Common.Enums.StatusCodes;

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

        [HttpGet]
        public IActionResult Error(StatusCodes statusCode, string message)
        {
            var viewModel = new ErrorViewModel
            {
                StatusCode = statusCode,
                Message = message,
            };

            return View(viewModel);
        }

        public IActionResult AccessDenied()
        {
            // Здесь можно добавить логику обработки ошибки, например, запись в лог или отображение пользователю сообщения об ошибке
            return View();
        }
    }
}