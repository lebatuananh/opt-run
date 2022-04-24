using Microsoft.AspNetCore.Authorization;
using RunOtp.Infrastructure;
using RunOtp.WebApi.Data.Migrations.RunOtp;
using RunOtp.WebApi.UseCase.WebConfigurations;

namespace RunOtp.WebApi.Controllers;

public class WebConfigurationController : BaseController
{
    [HttpGet]
    [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
    public async Task<IResult> GetPaging([FromQuery] int skip, int take, string query)
    {
        return await Mediator.Send(new MutateWebConfiguration.GetListWebConfigurationQueries(skip, take, query));
    }

    [HttpPost]
    [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
    public async Task<IResult> Create(MutateWebConfiguration.CreateWebConfigurationCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
    public async Task<IResult> GetById(Guid id)
    {
        return await Mediator.Send(new MutateWebConfiguration.GetWebConfigurationQuery() with { Id = id });
    }

    [HttpPost]
    [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
    public async Task<IResult> Update(MutateWebConfiguration.UpdateWebConfigurationCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpPost]
    [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
    public async Task<IResult> ChangeSelected(SelectWebConfiguration.ChangeSelectedCommand command)
    {
        return await Mediator.Send(command);
    }
}