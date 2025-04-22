using KOP.BLL.Interfaces;
using KOP.Common.Dtos.AccountDtos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
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
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            // Очищаем сессии
            HttpContext.Session.Clear();

            // Выходим из системы
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Перенаправляем на страницу входа
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.Login) || string.IsNullOrEmpty(dto.Password))
            {
                return BadRequest(new { error = "Логин и пароль не могут быть пустыми." });
            }

            var loginResponse = await _accountService.Login(dto);

            if (loginResponse.StatusCode != Common.Enums.StatusCodes.OK || loginResponse.Data == null)
            {
                return Unauthorized(new { error = loginResponse.Description });
            }

            var claimsPrincipal = loginResponse.Data;

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(loginResponse.Data),
                new AuthenticationProperties { IsPersistent = true });

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> RemindPassword([FromBody] LoginDto dto)
        {
            var remindPasswordResponse = await _accountService.RemindPassword(dto);

            if (remindPasswordResponse.StatusCode != Common.Enums.StatusCodes.OK)
            {
                return Unauthorized(new { error = remindPasswordResponse.Description });
            }

            return Ok(new { message = remindPasswordResponse.Description });
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}