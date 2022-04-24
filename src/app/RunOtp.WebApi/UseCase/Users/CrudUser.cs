using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity;
using RunOtp.Domain.OrderHistory;
using RunOtp.Domain.UserAggregate;

namespace RunOtp.WebApi.UseCase.Users;

public struct MutateUser
{
    public record GetListUserQueries(int Skip, int Take, string Query) : IQueries;

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
                        .WithMessage("Id value cannot be null");
                }
            }
        }
    }

    internal class Handler :
        IRequestHandler<RegisterUserCommand, IResult>,
        IRequestHandler<GetUserQuery, IResult>,
        IRequestHandler<GetListUserQueries, IResult>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IScopeContext _scopeContext;

        public Handler(UserManager<AppUser> userManager, IScopeContext scopeContext)
        {
            _userManager = userManager;
            _scopeContext = scopeContext;
        }

        public async Task<IResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var findByEmail = await _userManager.FindByEmailAsync(request.Email);
            var findByUsername = await _userManager.FindByNameAsync(request.UserName);
            if (findByEmail != null || findByUsername != null)
            {
                throw new Exception("User already exists");
            }

            var result = await _userManager.CreateAsync(new AppUser(request.UserName, request.Email, request.FullName,
                request.BirthDay, string.Empty, UserStatus.InActive), request.Password);

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
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
            if (user is null)
            {
                throw new Exception("User not found");
            }

            var userDto = new UserDto(user.Id, user.Email, user.UserName, user.Balance, user.TotalAmountUsed,
                user.Deposit, user.Discount, user.ClientSecret, user.Status);
            return Results.Ok(ResultModel<UserDto>.Create(userDto));
        }

        public async Task<IResult> Handle(GetListUserQueries request, CancellationToken cancellationToken)
        {
            var queryResult = await _userManager.Users.Include(x => x.OrderHistories).Where(x =>
                    string.IsNullOrEmpty(x.UserName) || EF.Functions.ILike(x.UserName, $"%{request.Query}%"))
                .OrderByDescending(x => x.Balance).ToQueryResultAsync(request.Skip, request.Take);

            var resultItems = queryResult.Items.Select(x =>
            {
                var totalRequest = x.OrderHistories.Count;
                var totalSuccessRequest = x.OrderHistories.Count(u => u.Status == OrderStatus.Success);
                var totalErrorRequest = x.OrderHistories.Count(u => u.Status == OrderStatus.Error);
                return new UserPagingDto(x.Id, x.Email, x.UserName, x.Balance,
                    x.TotalAmountUsed,
                    x.Deposit, x.Discount, x.ClientSecret, totalRequest, totalSuccessRequest, totalErrorRequest,
                    x.Status);
            });

            var result = new QueryResult<UserPagingDto>()
            {
                Count = queryResult.Count,
                Items = resultItems.ToList()
            };
            return Results.Ok(ResultModel<QueryResult<UserPagingDto>>.Create(result));
        }
    }
}