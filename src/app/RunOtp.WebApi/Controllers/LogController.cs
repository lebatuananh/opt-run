using Microsoft.AspNetCore.Authorization;
using RunOtp.WebApi.UseCase.ErrorLog;

namespace RunOtp.WebApi.Controllers;

public class LogController : BaseController
{
    [HttpGet]
    [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
    public async Task<IResult> GetPaging([FromQuery] int skip, int take, string query)
    {
        return await Mediator.Send(new MutateErrorLog.GetListErrorLogQueries(skip, take, query));
    }

    [HttpPost]
    [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
    public async Task<IResult> Delete(MutateErrorLog.DeleteLogsOlderThanCommand command)
    {
        return await Mediator.Send(command);
    }
}