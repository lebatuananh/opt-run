using RunOtp.Domain.WebConfigurationAggregate;
using RunOtp.WebApi.Services;

namespace RunOtp.WebApi.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class OtpExternalController : ControllerBase
{
    private readonly IOtpExternalService _otpExternalService;

    public OtpExternalController(IOtpExternalService otpExternalService)
    {
        _otpExternalService = otpExternalService;
    }

    [HttpGet]
    public async Task<IActionResult> CreateRequest(string apiKey, WebType webType)
    {
        return Ok(await _otpExternalService.CreateOtpRequest(apiKey, webType));
    }

    [HttpGet]
    public async Task<IActionResult> CheckRequest(string apiKey, WebType webType, Guid requestId)
    {
        return Ok(await _otpExternalService.CheckOtpRequest(apiKey, webType, requestId));
    }
}