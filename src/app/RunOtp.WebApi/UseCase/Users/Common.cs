using RunOtp.Domain.UserAggregate;

namespace RunOtp.WebApi.UseCase.Users;

public record UserDto(
    Guid Id,
    string Email,
    string Username,
    decimal Balance,
    decimal TotalAmountUsed,
    decimal Deposit,
    int Discount,
    string ClientSecret,
    UserStatus Status
    )
{
    // public List<TransactionDto> TransactionDtos { set; get; }
    // public List<OrderHistoryDto> OrderHistoryDtos { set; get; }
    //
    // public UserDto Assign(IList<Transaction> transactions, IList<OrderHistory> orderHistories)
    // {
    //     if (transactions is { Count: > 0 })
    //     {
    //         TransactionDtos = transactions.Select(x =>
    //             new TransactionDto(x.Id, x.TotalAmount, x.Action, x.BankAccount, x.CompletedDate, x.PaymentGateway,
    //                 x.CreatedDate, x.LastUpdatedDate)).ToList();
    //     }
    //
    //     if (orderHistories is { Count: > 0 })
    //     {
    //         OrderHistoryDtos = orderHistories.Select(x => new OrderHistoryDto(x.Id, x.NumberPhone, x.Message, x.WebType,
    //             x.Status, x.OtpCode, x.CreatedDate, x.LastUpdatedDate)).ToList();
    //     }
    //
    //     return this;
    // }
}

public record ReportDto(
    decimal TotalBalance,
    decimal TotalRechargeToday,
    int TotalRequest,
    int TotalRequestSuccess,
    int TotalRequestError);