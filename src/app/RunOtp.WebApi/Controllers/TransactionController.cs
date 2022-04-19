using RunOtp.WebApi.UseCase.Transactions;

namespace RunOtp.WebApi.Controllers;

public class TransactionController : BaseController
{
    [HttpGet]
    public async Task<IResult> GetPaging([FromQuery] int skip, int take, string query)
    {
        return await Mediator.Send(new MutateTransaction.GetListTransactionQueries(skip, take, query));
    }
}