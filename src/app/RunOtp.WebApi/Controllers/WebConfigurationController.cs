using RunOtp.WebApi.UseCase.WebConfigurations;

namespace RunOtp.WebApi.Controllers;

public class WebConfigurationController : BaseController
{
    [HttpGet]
    public async Task<IResult> GetPaging([FromQuery] int skip, int take, string query)
    {
        return await Mediator.Send(new MutateWebConfiguration.GetListWebConfigurationQueries(skip, take, query));
    }

    [HttpPost]
    public async Task<IResult> Create(MutateWebConfiguration.CreateWebConfigurationCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpGet("{id}")]
    public async Task<IResult> GetById(Guid id)
    {
        return await Mediator.Send(new MutateWebConfiguration.GetWebConfigurationQuery() with { Id = id });
    }

    [HttpPost]
    public async Task<IResult> Update(MutateWebConfiguration.UpdateWebConfigurationCommand command)
    {
        return await Mediator.Send(command);
    }
}