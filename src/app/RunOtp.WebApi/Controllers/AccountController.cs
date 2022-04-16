using Microsoft.AspNetCore.Authorization;
using RunOtp.WebApi.UseCase.Users;

namespace RunOtp.WebApi.Controllers;

public class AccountController : BaseController
{
    [AllowAnonymous]
    [HttpPost]
    public async Task<IResult> Login([FromBody] Token.CreateTokenCommand request)
    {
        return await Mediator.Send(request);
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IResult> Register(MutateUser.RegisterUserCommand request)
    {
        return await Mediator.Send(request);
    }
}