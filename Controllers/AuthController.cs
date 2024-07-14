using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using AspNet.Security.OAuth.GitHub;
using FeedbackApp.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Authorization;

namespace FeedbackApp.Controllers
{
    public class AuthController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> SignIn() => View("~/Views/Home/Index.cshtml", await HttpContext.GetExternalProvidersAsync());

        [HttpPost]
        public async Task<IActionResult> SignIn([FromForm] string provider)
        {
            if (string.IsNullOrWhiteSpace(provider))
            {
                return new BadRequestResult();
            }

            if (!await HttpContext.IsProviderSupportedAsync(provider))
            {
                return new BadRequestResult();
            }

            return Challenge(new AuthenticationProperties { RedirectUri = "/" }, provider);
        }

        [HttpGet]
        [HttpPost]
        public IActionResult SignOutCurrentUser()
        {
            return SignOut(new AuthenticationProperties { RedirectUri = "/"}, CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
