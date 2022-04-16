using RunOtp.WebApi.UseCase.OrderHistories;

namespace RunOtp.WebApi.Controllers;

public class OrderHistoryController : BaseController
{
    [HttpPost]
    public async Task<IResult> Create()
    {
        return await Mediator.Send(new MutateOrderHistory.CreateOrderHistoryCommand());
    }

    [HttpGet]
    public async Task<IResult> GetPaging([FromQuery] int skip, int take, string query)
    {
        return await Mediator.Send(new MutateOrderHistory.GetListOrderHistoryQueries(skip, take, query));
    }
}