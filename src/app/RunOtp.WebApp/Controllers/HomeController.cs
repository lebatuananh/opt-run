using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RunOtp.WebApp.Models;
using Shared.Extensions;

namespace RunOtp.WebApp.Controllers;

public class HomeController : BaseController
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        var email = User.GetSpecificClaim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}