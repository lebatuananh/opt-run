using RunOtp.Domain.OrderHistory;
using RunOtp.Domain.TransactionAggregate;
using RunOtp.Domain.UserAggregate;
using RunOtp.Domain.WebConfigurationAggregate;
using Action = RunOtp.Domain.TransactionAggregate.Action;


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
}

public record UserPagingDto(
    Guid Id,
    string Email,
    string Username,
    decimal Balance,
    decimal TotalAmountUsed,
    decimal Deposit,
    int Discount,
    string ClientSecret,
    int TotalRequest,
    int TotalSuccess,
    int TotalFailed,
    UserStatus Status
)
{
}

public record ReportDto(
    decimal TotalRecharge,
    decimal TotalBalance,
    decimal TotalRechargeToday,
    int TotalRequest,
    int TotalRequestSuccess,
    int TotalRequestError);

public record TotalOrderHistoryDto(
    int TotalTransaction,
    int TotalRequest,
    int TotalRequestSuccess,
    int TotalRequestError
);