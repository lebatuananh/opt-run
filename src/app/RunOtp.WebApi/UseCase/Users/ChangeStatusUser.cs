using Microsoft.AspNetCore.Identity;
using RunOtp.Domain.UserAggregate;

namespace RunOtp.WebApi.UseCase.Users;

public struct ChangeStatusUser
{
    public record ActiveUserCommand : IUpdateCommand<Guid>
    {
        public Guid Id { get; init; }

        internal class GetValidator : AbstractValidator<ActiveUserCommand>
        {
            public GetValidator()
            {
                RuleFor(v => v.Id)
                    .NotEmpty()
                    .WithMessage("Id value cannot be null");
            }
        }
    }

    public record InActiveUserCommand : IUpdateCommand<Guid>
    {
        public Guid Id { get; init; }

        internal class GetValidator : AbstractValidator<InActiveUserCommand>
        {
            public GetValidator()
            {
                RuleFor(v => v.Id)
                    .NotEmpty()
                    .WithMessage("Id value cannot be null");
            }
        }
    }

    public record UpdateDiscountCommand : IUpdateCommand<Guid>
    {
        public Guid Id { get; init; }
        public int Discount { get; set; }

        internal class GetValidator : AbstractValidator<UpdateDiscountCommand>
        {
            public GetValidator()
            {
                RuleFor(v => v.Id)
                    .NotEmpty()
                    .WithMessage("Id value cannot be null");
                RuleFor(v => v.Discount)
                    .GreaterThan(0)
                    .WithMessage("Promotion price must be greater than 0");
            }
        }
    }

    internal class Handler :
        IRequestHandler<ActiveUserCommand, IResult>,
        IRequestHandler<InActiveUserCommand, IResult>,
        IRequestHandler<UpdateDiscountCommand, IResult>
    {
        private readonly UserManager<AppUser> _userManager;

        public Handler(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IResult> Handle(ActiveUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager
                .Users
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
            if (user is null)
            {
                throw new Exception("User not found");
            }

            user.Enable();
            await _userManager.UpdateAsync(user);
            return Results.Ok();
        }

        public async Task<IResult> Handle(InActiveUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager
                .Users
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
            if (user is null)
            {
                throw new Exception("User not found");
            }

            user.Disable();
            await _userManager.UpdateAsync(user);
            return Results.Ok();
        }

        public async Task<IResult> Handle(UpdateDiscountCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager
                .Users
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
            if (user is null)
            {
                throw new Exception("User not found");
            }

            user.UpdateDiscount(request.Discount);
            await _userManager.UpdateAsync(user);
            return Results.Ok();
        }
    }
}