namespace RunOtp.WebApp.Controllers.Components;

public class FooterViewComponent : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        return View();
    }
}