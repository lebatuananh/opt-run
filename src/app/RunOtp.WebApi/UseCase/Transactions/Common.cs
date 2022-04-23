using RunOtp.Domain.TransactionAggregate;
using Action = RunOtp.Domain.TransactionAggregate.Action;

namespace RunOtp.WebApi.UseCase.Transactions;

public record TransactionDto(Guid Id,
    decimal TotalAmount,
    Action Action,
    string BankAccount,
    string Note,
    string Response,
    string Email,
    TransactionStatus Status,
    DateTimeOffset CompletedDate,
    PaymentGateway PaymentGateway,
    DateTimeOffset CreatedDate,
    DateTimeOffset LastUpdatedDate);