using Microsoft.AspNetCore.Identity;
using RunOtp.Domain.OrderHistory;
using RunOtp.Domain.TransactionAggregate;
using RunOtp.Domain.UserAggregate;
using RunOtp.Infrastructure;
using Action = RunOtp.Domain.TransactionAggregate.Action;

namespace RunOtp.WebApi.UseCase.Users;

public struct Report
{
    public record GetReportQuery : IQuery;

    internal class Handler : IRequestHandler<GetReportQuery, IResult>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IOrderHistoryRepository _orderHistoryRepository;
        private readonly IScopeContext _scopeContext;

        public Handler(UserManager<AppUser> userManager, ITransactionRepository transactionRepository,
            IOrderHistoryRepository orderHistoryRepository, IScopeContext scopeContext)
        {
            _userManager = userManager;
            _transactionRepository = transactionRepository;
            _orderHistoryRepository = orderHistoryRepository;
            _scopeContext = scopeContext;
        }

        public Task<IResult> Handle(GetReportQuery request, CancellationToken cancellationToken)
        {
            decimal totalRecharge;
            decimal totalBalance;
            decimal rechargeToday;
            int totalRequest;
            int requestSuccess;
            int requestFailed;
            if (_scopeContext.Role == SystemConstants.Admin)
            {
                totalRecharge = _userManager.Users.Sum(x => x.Deposit);
                totalBalance = _userManager.Users.Sum(x => x.Balance);
                rechargeToday =
                    _transactionRepository.FindAll(x => string.IsNullOrEmpty(x.Ref) && x.Action == Action.Recharge)
                        .Sum(x => x.TotalAmount);
                totalRequest = _orderHistoryRepository.FindAll().Count();
                requestSuccess = _orderHistoryRepository.FindAll(x => x.Status == OrderStatus.Success).Count();
                requestFailed = _orderHistoryRepository.FindAll(x => x.Status == OrderStatus.Error).Count();
                var result = new ReportDto(totalRecharge, totalBalance, rechargeToday, totalRequest, requestSuccess,
                    requestFailed);
                return Task.FromResult(Results.Ok(ResultModel<ReportDto>.Create(result)));
            }

            totalRecharge = _userManager.Users.Where(x => x.Id == _scopeContext.CurrentAccountId).Sum(x => x.Deposit);
            totalBalance = _userManager.Users.Where(x => x.Id == _scopeContext.CurrentAccountId).Sum(x => x.Balance);
            rechargeToday =
                _transactionRepository.FindAll(x =>
                        string.IsNullOrEmpty(x.Ref) && x.Action == Action.Recharge &&
                        x.UserId == _scopeContext.CurrentAccountId)
                    .Sum(x => x.TotalAmount);
            totalRequest = _orderHistoryRepository.FindAll(x => x.UserId == _scopeContext.CurrentAccountId).Count();
            requestSuccess = _orderHistoryRepository
                .FindAll(x => x.Status == OrderStatus.Success && x.UserId == _scopeContext.CurrentAccountId).Count();
            requestFailed = _orderHistoryRepository
                .FindAll(x => x.Status == OrderStatus.Error && x.UserId == _scopeContext.CurrentAccountId).Count();
            var resultCurrent =
                new ReportDto(totalRecharge, totalBalance, rechargeToday, totalRequest, requestSuccess, requestFailed);
            return Task.FromResult(Results.Ok(ResultModel<ReportDto>.Create(resultCurrent)));
        }
    }
}