using KOP.BLL.Interfaces;
using KOP.Common.DTOs.AccountDTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KOP.WEB.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        public async Task<JsonResult> Login([FromBody] LoginDTO dto)
        {
            var response = await _accountService.Login(dto);

            if (!response.HasData)
            {
                return Json(new { description = response.Description, statusCode = (int)response.StatusCode });
            }

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                   new ClaimsPrincipal(response.Data),
                   new AuthenticationProperties { IsPersistent = true });

            return Json(new { statusCode = (int)response.StatusCode });
        }

        [HttpPost]
        public async Task<JsonResult> RemindPassword([FromBody] LoginDTO dto)
        {
            var response = await _accountService.RemindPassword(dto);

            return Json(new { description = response.Description, statusCode = (int)response.StatusCode });
        }
    }
}