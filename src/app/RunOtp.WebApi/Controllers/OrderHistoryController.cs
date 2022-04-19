using RunOtp.WebApi.UseCase.OrderHistories;

namespace RunOtp.WebApi.Controllers;

public class OrderHistoryController : BaseController
{
    [HttpPost]
    public async Task<IResult> Create(MutateOrderHistory.CreateOrderHistoryCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpGet]
    public async Task<IResult> GetPaging([FromQuery] int skip, int take, string query)
    {
        return await Mediator.Send(new MutateOrderHistory.GetListOrderHistoryQueries(skip, take, query));
    }
}