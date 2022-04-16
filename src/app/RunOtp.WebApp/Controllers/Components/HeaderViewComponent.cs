namespace RunOtp.WebApp.Controllers.Components;

public class HeaderViewComponent: ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        return View();
    }
}