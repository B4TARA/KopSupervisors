using System.Security.Claims;
using KOP.BLL.Interfaces;
using KOP.Common.Dtos.AccountDtos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

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
            if (User.Identity != null && User.Identity.IsAuthenticated)
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
        public async Task<IActionResult> LoginNow(LoginDto dto)
        {
            if (ModelState.IsValid)
            {
                var response = await _accountService.LoginNow(dto);

                if (response.HasData)
                {
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(response.Data),
                        new AuthenticationProperties { IsPersistent = true });

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", response.Description);
            }

            return View("Login", dto);
        }

        [HttpPost]
        public async Task<JsonResult> Login([FromBody] LoginDto dto)
        {
            var response = await _accountService.Login(dto);

            if (response.StatusCode == Common.Enums.StatusCodes.Redirect)
            {
                return Json(new { statusCode = (int)response.StatusCode });
            }

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
        public async Task<JsonResult> RemindPassword([FromBody] LoginDto dto)
        {
            var response = await _accountService.RemindPassword(dto);

            return Json(new { description = response.Description, statusCode = (int)response.StatusCode });
        }
    }
}