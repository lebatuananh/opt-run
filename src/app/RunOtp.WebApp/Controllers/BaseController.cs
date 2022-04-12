using Microsoft.AspNetCore.Authorization;

namespace RunOtp.WebApp.Controllers;

[Authorize]
public class BaseController : Controller
{
}