using Microsoft.AspNetCore.Identity;
using RunOtp.Domain.TransactionAggregate;
using RunOtp.Domain.UserAggregate;
using Action = RunOtp.Domain.TransactionAggregate.Action;

namespace RunOtp.WebApi.UseCase.OrderHistories;

public struct Wallet
{
    public record RechargeCommand : ICreateCommand
    {
        public decimal TotalAmount { get; set; }
        public string Note { get; set; }
        public string BankAccount { get; set; }
        public Guid UserId { get; set; }

        internal class GetValidator : AbstractValidator<RechargeCommand>
        {
            public GetValidator()
            {
                RuleFor(v => v.TotalAmount)
                    .GreaterThan(0);
                RuleFor(v => v.UserId)
                    .NotEmpty()
                    .WithMessage("Id value cannot be null");
            }
        }
    }

    public record DeductionCommand : ICreateCommand
    {
        public decimal TotalAmount { get; set; }
        public string Note { get; set; }
        public string BankAccount { get; set; }
        public Guid UserId { get; set; }

        internal class GetValidator : AbstractValidator<DeductionCommand>
        {
            public GetValidator()
            {
                RuleFor(v => v.TotalAmount)
                    .GreaterThan(0);
                RuleFor(v => v.UserId)
                    .NotEmpty()
                    .WithMessage("Id value cannot be null");
            }
        }
    }

    internal class Handler : IRequestHandler<RechargeCommand, IResult>, IRequestHandler<DeductionCommand, IResult>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITransactionRepository _transactionRepository;

        public Handler(UserManager<AppUser> userManager, ITransactionRepository transactionRepository)
        {
            _userManager = userManager;
            _transactionRepository = transactionRepository;
        }

        public async Task<IResult> Handle(RechargeCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
            {
                throw new Exception("User not found");
            }
            user.Recharge(request.TotalAmount);
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) throw new Exception("Đã có lỗi xảy ra");
            var entity = new Transaction(request.UserId, request.TotalAmount, request.Note,
                request.BankAccount, PaymentGateway.BankTransfer, Action.Recharge);
            entity.MarkCompleted();
            _transactionRepository.Add(entity);
            await _transactionRepository.CommitAsync();

            return Results.Ok();
        }

        public async Task<IResult> Handle(DeductionCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
            {
                throw new Exception("User not found");
            }
            user.SubtractMoney(request.TotalAmount);
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) throw new Exception("Đã có lỗi xảy ra");
            var entity = new Transaction(request.UserId, request.TotalAmount, request.Note,
                request.BankAccount, PaymentGateway.BankTransfer, Action.Deduction);
            entity.MarkCompleted();
            _transactionRepository.Add(entity);
            await _transactionRepository.CommitAsync();

            return Results.Ok();
        }
    }
}