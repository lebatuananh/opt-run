using RunOtp.Domain.OrderHistory;
using RunOtp.Domain.TransactionAggregate;
using RunOtp.Domain.WebConfigurationAggregate;
using RunOtp.Infrastructure;
using Action = RunOtp.Domain.TransactionAggregate.Action;

namespace RunOtp.WebApi.UseCase.Transactions;

public struct MutateTransaction
{
    public record GetListTransactionQueries(int Skip, int Take, string Query) : IQueries;

    internal class Handler : IRequestHandler<GetListTransactionQueries, IResult>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IOrderHistoryRepository _orderHistoryRepository;
        private readonly IScopeContext _scopeContext;

        public Handler(ITransactionRepository transactionRepository, IScopeContext scopeContext,
            IOrderHistoryRepository orderHistoryRepository)
        {
            _transactionRepository = transactionRepository;
            _scopeContext = scopeContext;
            _orderHistoryRepository = orderHistoryRepository;
        }

        public async Task<IResult> Handle(GetListTransactionQueries request, CancellationToken cancellationToken)
        {
            QueryResult<Transaction> queryable;
            if (_scopeContext.Role.Equals(SystemConstants.Admin))
            {
                var queryAll = _transactionRepository
                    .FindAll(x => x.AppUser);
                if (!string.IsNullOrEmpty(request.Query))
                {
                    queryable = await queryAll.Where(x =>
                            x.AppUser.UserName == request.Query || EF.Functions.ILike(x.Note, $"%{request.Query}%"))
                        .OrderByDescending(x => x.CreatedDate)
                        .ToQueryResultAsync(request.Skip, request.Take);
                }
                else
                {
                    queryable = await queryAll
                        .OrderByDescending(x => x.CreatedDate)
                        .ToQueryResultAsync(request.Skip, request.Take);
                }
            }
            else
            {
                var queryCurrentUser = _transactionRepository
                    .FindAll(x => x.UserId == _scopeContext.CurrentAccountId, x => x.AppUser);
                if (!string.IsNullOrEmpty(request.Query))
                {
                    queryable = await queryCurrentUser.Where(x => EF.Functions.ILike(x.Note, $"%{request.Query}%"))
                        .OrderByDescending(x => x.CreatedDate)
                        .ToQueryResultAsync(request.Skip, request.Take);
                }
                else
                {
                    queryable = await queryCurrentUser
                        .OrderByDescending(x => x.CreatedDate)
                        .ToQueryResultAsync(request.Skip, request.Take);
                }
            }

            var result = new QueryResult<TransactionDto>
            {
                Count = queryable.Count,
                Items = queryable.Items
                    .Select(x => new TransactionDto(
                        x.Id,
                        x.TotalAmount,
                        x.Action,
                        x.BankAccount,
                        x.Note,
                        x.Response,
                        x.AppUser.UserName,
                        x.Status,
                        x.CompletedDate,
                        x.PaymentGateway,
                        x.CreatedDate,
                        x.LastUpdatedDate))
                    .ToList()
            };
            return Results.Ok(ResultModel<QueryResult<TransactionDto>>.Create(result));
        }
    }
}