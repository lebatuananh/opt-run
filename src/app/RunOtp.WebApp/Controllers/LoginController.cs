using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using RunOtp.Domain.UserAggregate;
using RunOtp.WebApp.Models;
using RunOtp.WebApp.Models.AccountViewModels;

namespace RunOtp.WebApp.Controllers;

public class LoginController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    public LoginController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }
    [HttpGet]
    [AllowAnonymous]
    [Route("login.html", Name = "Login")]
    public async Task<IActionResult> Index(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Authen(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe,
                lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return new OkObjectResult(new GenericResult(true));
            }

            if (result.IsLockedOut)
            {
                return new ObjectResult(new GenericResult(false, "Tài khoản đã bị khoá"));
            }
            else
            {
                return new ObjectResult(new GenericResult(false, "Đăng nhập sai"));
            }
        }

        // If we got this far, something failed, redisplay form
        return new ObjectResult(new GenericResult(false, model));
    }
}