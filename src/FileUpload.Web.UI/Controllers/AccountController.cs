using FileUpload.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
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
        [Route("[action]")]
        public IActionResult Login() => View();

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (model.Username == "test" && model.Password == "test")
            {
                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, model.Username),
                    new Claim(ClaimTypes.Role, "test"),
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme
                );

                var authProperties = new AuthenticationProperties()
                {
                    AllowRefresh = true
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties
                );
            }

            return RedirectTo();
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
            string returnUrl = HttpContext.Request.Query[CookieAuthenticationDefaults.ReturnUrlParameter];
            if (returnUrl != null)
                return Redirect(returnUrl);

            return RedirectToAction(nameof(MainController.Index), "Main");
        }
    }
}
