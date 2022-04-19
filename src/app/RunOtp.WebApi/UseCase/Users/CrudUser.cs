using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity;
using RunOtp.Domain.RoleAggregate;
using RunOtp.Domain.UserAggregate;

namespace RunOtp.WebApi.UseCase.Users;

public struct MutateUser
{
    public record GetListUserQueries(int Skip, int Take, string? Query) : IQueries;

    public record RegisterUserCommand : ICreateCommand
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public DateTime? BirthDay { set; get; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        internal class GetValidator : AbstractValidator<RegisterUserCommand>
        {
            public GetValidator()
            {
                RuleFor(x => x.FullName)
                    .NotNull()
                    .NotEmpty()
                    .WithMessage("Name is not empty");
                RuleFor(x => x.Password)
                    .NotEmpty()
                    .NotNull()
                    .WithMessage("Password is not empty");
                RuleFor(x => x.ConfirmPassword)
                    .NotEmpty()
                    .NotNull()
                    .WithMessage("ConfirmPassword is not empty");
                RuleFor(customer => customer.Password)
                    .Equal(customer => customer.ConfirmPassword).WithMessage("Password miss match");
            }
        }
    }

    public record GetUserQuery : IQuery
    {
        public Guid Id { get; init; }

        internal class GetSpec : SpecificationBase<AppUser>
        {
            private readonly Guid _id;

            public GetSpec(Guid id)
            {
                _id = id;
            }

            public override Expression<Func<AppUser, bool>> Criteria => x => x.Id == _id;

            internal class GetValidator : AbstractValidator<GetUserQuery>
            {
                public GetValidator()
                {
                    RuleFor(v => v.Id)
                        .NotEmpty()
                        .WithMessage("Giá trị Id không được null");
                }
            }
        }
    }

    internal class Handler : IRequestHandler<RegisterUserCommand, IResult>,
        IRequestHandler<GetUserQuery, IResult>
    {
        private readonly RoleManager<AppRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public Handler(RoleManager<AppRole> roleManager, UserManager<AppUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<IResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var findByEmail = await _userManager.FindByEmailAsync(request.Email);
            var findByUsername = await _userManager.FindByNameAsync(request.UserName);
            if (findByEmail != null || findByUsername != null)
            {
                throw new Exception("Người dùng đã tồn tại");
            }

            var result = await _userManager.CreateAsync(new AppUser(request.UserName, request.Email, request.FullName,
                request.BirthDay, string.Empty, UserStatus.Active), request.Password);

            if (!result.Succeeded) throw new Exception("Đăng ký không thành công");
            var appUser = await _userManager.FindByNameAsync(request.UserName);
            if (appUser != null)
                await _userManager.AddToRoleAsync(appUser, "Customer");
            return Results.Ok();
        }

        public async Task<IResult> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager
                .Users
                .Include(x => x.Transactions)
                .Include(x => x.OrderHistories)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
            if (user is null)
            {
                throw new Exception("Người dùng không tồn tại");
            }

            var userDto = new UserDto(user.Id, user.Email, user.UserName, user.Balance, user.TotalAmountUsed,
                user.Deposit, user.Discount, user.ClientSecret);
            return Results.Ok(ResultModel<UserDto>.Create(userDto));
        }
    }
}