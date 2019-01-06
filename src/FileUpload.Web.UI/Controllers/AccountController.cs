using FileUpload.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Neptuo;
using Neptuo.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FileUpload.Controllers
{
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly AccountOptions options;

        public AccountController(IOptions<AccountOptions> options)
        {
            Ensure.NotNull(options, "options");
            this.options = options.Value;
        }

        [Route("[action]")]
        public IActionResult Login() => View();

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                string password = HashProvider.Sha256($"{model.Username}.{model.Password}");

                AccountModel account = options.Accounts.FirstOrDefault(a => a.Username == model.Username && a.Password == password);
                if (account == null)
                {
                    ModelState.AddModelError(nameof(LoginModel.Username), "No such combination of the username and the password.");
                    return View();
                }

                ClaimsPrincipal principal = CreatePrincipal(account);
                AuthenticationProperties authProperties = CreateAuthenticationProperties();
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    authProperties
                );

                return RedirectTo();
            }

            return View();
        }

        private static AuthenticationProperties CreateAuthenticationProperties()
        {
            return new AuthenticationProperties()
            {
                AllowRefresh = true
            };
        }

        private static ClaimsPrincipal CreatePrincipal(AccountModel account)
        {
            var claims = new List<Claim>(1 + account.Roles?.Count ?? 0);

            claims.Add(new Claim(ClaimTypes.Name, account.Username));

            if (account.Roles != null)
            {
                foreach (string role in account.Roles)
                    claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            return new ClaimsPrincipal(claimsIdentity);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectTo();
        }

        private IActionResult RedirectTo()
        {
            string returnUrl = HttpContext.Request.Form[CookieAuthenticationDefaults.ReturnUrlParameter];
            if (!String.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction(nameof(MainController.Index), "Main");
        }
    }
}
