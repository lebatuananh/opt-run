using Microsoft.AspNetCore.Authorization;
using RunOtp.WebApi.UseCase.OrderHistories;
using RunOtp.WebApi.UseCase.Users;

namespace RunOtp.WebApi.Controllers;

public class AccountController : BaseController
{
    private readonly IScopeContext _scopeContext;

    public AccountController(IScopeContext scopeContext)
    {
        _scopeContext = scopeContext;
    }

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

    [HttpGet]
    public async Task<IResult> GetCurrentUser()
    {
        return await Mediator.Send(new MutateUser.GetUserQuery() { Id = _scopeContext.CurrentAccountId });
    }

    [HttpPost]
    [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
    public async Task<IResult> Recharge(Wallet.RechargeCommand rechargeCommand)
    {
        return await Mediator.Send(rechargeCommand);
    }

    [HttpPost]
    [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
    public async Task<IResult> Deduction(Wallet.DeductionCommand deductionCommand)
    {
        return await Mediator.Send(deductionCommand);
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
    public async Task<IResult> GetPaging([FromQuery] int skip, int take, string query)
    {
        return await Mediator.Send(new MutateUser.GetListUserQueries(skip, take, query));
    }

    [HttpPost]
    [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
    public async Task<IResult> Active(ChangeStatusUser.ActiveUserCommand activeUserCommand)
    {
        return await Mediator.Send(activeUserCommand);
    }

    [HttpPost]
    [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
    public async Task<IResult> InActive(ChangeStatusUser.InActiveUserCommand inActiveUserCommand)
    {
        return await Mediator.Send(inActiveUserCommand);
    }

    [HttpPost]
    [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
    public async Task<IResult> UpdateDiscount(ChangeStatusUser.UpdateDiscountCommand updateDiscountCommand)
    {
        return await Mediator.Send(updateDiscountCommand);
    }

    [HttpGet]
    public async Task<IResult> Report()
    {
        return await Mediator.Send(new Report.GetReportQuery());
    }
}