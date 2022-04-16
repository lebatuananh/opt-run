namespace RunOtp.WebApp.Controllers;

public class UserController : BaseController
{
    public async Task<IActionResult> Index(int? pageSize, int page = 1)
    {
        return View();
    }
}