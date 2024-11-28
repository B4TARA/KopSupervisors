using KOP.BLL.Interfaces;
using KOP.Common.DTOs.AccountDTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using StatusCodes = KOP.Common.Enums.StatusCodes;

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

            var remindPasswordLogin = TempData["remindPasswordLogin"]?.ToString();
            var remindPasswordMessage = TempData["remindPasswordMessage"]?.ToString();

            if (!string.IsNullOrEmpty(remindPasswordLogin))
            {
                ViewBag.remindPasswordLogin = remindPasswordLogin;
            }

            if (!string.IsNullOrEmpty(remindPasswordMessage))
            {
                ModelState.AddModelError("", remindPasswordMessage);
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
        public async Task<IActionResult> Login(AccountDTO accountDTO)
        {
            if (ModelState.IsValid)
            {
                var response = await _accountService.Login(accountDTO);

                if (response.StatusCode == StatusCodes.OK && response.Data != null)
                {
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(response.Data),
                        new AuthenticationProperties { IsPersistent = true });

                    return RedirectToRoute(new
                    {
                        httpMethod = "POST",
                        controller = "Home",
                        action = "Index",
                    });
                }
                else
                {
                    ModelState.AddModelError("", response.Description);
                }
            }

            return View(accountDTO);
        }

        [HttpPost]
        public async Task<IActionResult> RemindPassword(AccountDTO accountDTO)
        {
            TempData["remindPasswordLogin"] = accountDTO.Login;

            if (ModelState.IsValid)
            {
                var remindPasswordResponse = await _accountService.RemindPassword(accountDTO);

                if (remindPasswordResponse.StatusCode == StatusCodes.EntityNotFound)
                {
                    TempData["changePasswordMessage"] = "Пользователя с таким login-ом не существует";
                    return RedirectToAction("Login", "Account");
                }
                else if (remindPasswordResponse.StatusCode == StatusCodes.InternalServerError)
                {
                    TempData["changePasswordMessage"] = $"Произошла непредвиденная ошибка : {remindPasswordResponse.Description}";
                    return RedirectToAction("Login", "Account");
                }
                else if (remindPasswordResponse.StatusCode == StatusCodes.OK)
                {
                    TempData["changePasswordMessage"] = "Ваши учетные данные успешно высланы на привязанную к аккаунту почту";
                    return RedirectToAction("Login", "Account");
                }
            }

            TempData["changePasswordMessage"] = "Invalid ModelState";
            return RedirectToAction("Login", "Account");
        }
    }
}
