﻿using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity;
using RunOtp.Domain.OrderHistory;
using RunOtp.Domain.TransactionAggregate;
using RunOtp.Domain.UserAggregate;
using RunOtp.Domain.WebConfigurationAggregate;
using RunOtp.Driver;
using RunOtp.Driver.OtpTextNow;
using RunOtp.Driver.RentOtp;
using RunOtp.Driver.RunOtp;
using RunOtp.Infrastructure;
using RunOtp.WebApi.UseCase.WebConfigurations;

namespace RunOtp.WebApi.UseCase.OrderHistories;

public struct MutateOrderHistory
{
    public record GetListOrderHistoryQueries(int Skip, int Take, string Query) : IQueries;

    public record GetOrderHistoryQuery : IQuery
    {
        public Guid Id { get; init; }

        internal class GetSpec : SpecificationBase<OrderHistory>
        {
            private readonly Guid _id;

            public GetSpec(Guid id)
            {
                _id = id;
            }

            public override Expression<Func<OrderHistory, bool>> Criteria => x => x.Id == _id;

            internal class GetValidator : AbstractValidator<MutateWebConfiguration.GetWebConfigurationQuery>
            {
                public GetValidator()
                {
                    RuleFor(v => v.Id)
                        .NotEmpty()
                        .WithMessage("Bắt buộc phải nhập Id");
                }
            }
        }
    }

    public record CreateOrderHistoryCommand : ICreateCommand
    {
        public WebType WebType { get; set; }
    }

    internal class Handler : IRequestHandler<GetListOrderHistoryQueries, IResult>,
        IRequestHandler<GetOrderHistoryQuery, IResult>, IRequestHandler<CreateOrderHistoryCommand, IResult>
    {
        private readonly IOrderHistoryRepository _orderHistoryRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly IScopeContext _scopeContext;
        private readonly IOtpTextNowClient _otpTextNowClient;
        private readonly IRunOtpClient _runOtpClient;
        private readonly IRentCodeTextNowClient _rentCodeTextNowClient;
        private readonly IWebConfigurationRepository _webConfigurationRepository;

        public Handler(IOrderHistoryRepository orderHistoryRepository, IOtpTextNowClient otpTextNowClient,
            IRunOtpClient runOtpClient, ITransactionRepository transactionRepository, IScopeContext scopeContext,
            UserManager<AppUser> userManager, IWebConfigurationRepository webConfigurationRepository,
            IRentCodeTextNowClient rentCodeTextNowClient)
        {
            _orderHistoryRepository = orderHistoryRepository;
            _otpTextNowClient = otpTextNowClient;
            _runOtpClient = runOtpClient;
            _transactionRepository = transactionRepository;
            _scopeContext = scopeContext;
            _userManager = userManager;
            _webConfigurationRepository = webConfigurationRepository;
            _rentCodeTextNowClient = rentCodeTextNowClient;
        }

        public async Task<IResult> Handle(GetListOrderHistoryQueries request, CancellationToken cancellationToken)
        {
            QueryResult<OrderHistory> queryable;
            if (_scopeContext.Role.Equals(SystemConstants.Admin))
            {
                var queryAll = _orderHistoryRepository
                    .FindAll(x => x.AppUser);
                if (!string.IsNullOrEmpty(request.Query))
                {
                    queryable = await queryAll.Where(x =>
                            x.AppUser.UserName == request.Query ||
                            EF.Functions.ILike(x.NumberPhone, $"%{request.Query}%"))
                        .OrderByDescending(x => x.CreatedDate)
                        .ToQueryResultAsync(request.Skip, request.Take);
                }
                else
                {
                    queryable = await queryAll
                        .OrderByDescending(x => x.CreatedDate).ToQueryResultAsync(request.Skip, request.Take);
                }
            }
            else
            {
                var queryCurrentUser = _orderHistoryRepository
                    .FindAll(x => x.UserId == _scopeContext.CurrentAccountId, x => x.AppUser);
                if (!string.IsNullOrEmpty(request.Query))
                {
                    queryable = await queryCurrentUser.Where(x =>
                            EF.Functions.ILike(x.NumberPhone, $"%{request.Query}%"))
                        .OrderByDescending(x => x.CreatedDate)
                        .ToQueryResultAsync(request.Skip, request.Take);
                }
                else
                {
                    queryable = await queryCurrentUser
                        .OrderByDescending(x => x.CreatedDate).ToQueryResultAsync(request.Skip, request.Take);
                }
            }

            var result = new QueryResult<OrderHistoryDto>
            {
                Count = queryable.Count,
                Items = queryable.Items
                    .Select(x => new OrderHistoryDto(
                        x.Id,
                        x.NumberPhone,
                        x.Message,
                        x.WebType,
                        x.Status,
                        x.OtpCode,
                        x.AppUser.UserName,
                        x.CreatedDate,
                        x.LastUpdatedDate))
                    .ToList()
            };
            return Results.Ok(ResultModel<QueryResult<OrderHistoryDto>>.Create(result));
        }

        public async Task<IResult> Handle(GetOrderHistoryQuery request, CancellationToken cancellationToken)
        {
            var item = await _orderHistoryRepository.GetByIdAsync(request.Id);
            if (item is null) throw new Exception($"Không tìm thấy bản ghi item={request.Id}");
            var result = new OrderHistoryDto(
                item.Id,
                item.NumberPhone,
                item.Message,
                item.WebType,
                item.Status,
                item.OtpCode,
                item.AppUser.UserName,
                item.CreatedDate,
                item.LastUpdatedDate);
            return Results.Ok(ResultModel<OrderHistoryDto>.Create(result));
        }

        public async Task<IResult> Handle(CreateOrderHistoryCommand request,
            CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(_scopeContext.CurrentAccountId.ToString());
            if (user is null)
            {
                throw new Exception("User does not exist, try re-entering apiKey");
            }

            if (user.Balance <= 0)
            {
                throw new Exception("Your account is not enough to use the service, please add more money");
            }

            var webTypeResult = await _webConfigurationRepository.GetSingleAsync(x => x.Selected);
            var webType = webTypeResult?.WebType ?? WebType.OtpTextNow;

            switch (webType)
            {
                case WebType.RunOtp:
                    var resultRunOtpResponse = await _runOtpClient.CreateRequest(_scopeContext.CurrentAccountId);
                    return Results.Ok(ResultModel<CreateOrderResponseClient>.Create(resultRunOtpResponse));
                case WebType.OtpTextNow:
                    var resultNumberResponse = await _otpTextNowClient.CreateRequest(_scopeContext.CurrentAccountId);
                    return Results.Ok(ResultModel<CreateOrderResponseClient>.Create(resultNumberResponse));
                case WebType.RentOtp:
                    var resultNumberRentResponse =
                        await _rentCodeTextNowClient.CreateRequest(_scopeContext.CurrentAccountId);
                    return Results.Ok(ResultModel<CreateOrderResponseClient>.Create(resultNumberRentResponse));
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return Results.Ok();
        }
    }
}