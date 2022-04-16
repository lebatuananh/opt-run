using RunOtp.Domain.UserAggregate;
using Shared.SeedWork;

namespace RunOtp.Domain.TransactionAggregate;

public class Transaction : ModifierTrackingEntity, IAggregateRoot
{
    public decimal TotalAmount { get; private set; }
    public string Note { get; private set; }
    public string ErrorMessage { get; private set; }
    public string BankAccount { get; private set; }
    public DateTimeOffset CompletedDate { get; private set; }
    public string Response { get; private set; }
    public PaymentGateway PaymentGateway { get; private set; }
    public TransactionStatus Status { get; private set; }
    public Guid UserId { get; private set; }
    public virtual AppUser AppUser { get; private set; }

    public Transaction()
    {
    }

    public Transaction(Guid userId, decimal totalAmount, string note, string bankAccount,
        PaymentGateway paymentGateway) : this()
    {
        TotalAmount = totalAmount;
        Note = note;
        BankAccount = bankAccount;
        UserId = userId;
        PaymentGateway = paymentGateway;
    }

    public void MarkCompleted()
    {
        CompletedDate = DateTimeOffset.UtcNow;
        Status = TransactionStatus.Completed;
    }
}