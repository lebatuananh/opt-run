﻿using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity;
using RunOtp.Domain.OrderHistory;
using RunOtp.Domain.TransactionAggregate;
using RunOtp.Domain.UserAggregate;
using RunOtp.Domain.WebConfigurationAggregate;
using RunOtp.Driver;
using RunOtp.Driver.OtpTextNow;
using RunOtp.Driver.RunOtp;
using RunOtp.WebApi.UseCase.WebConfigurations;
using Action = RunOtp.Domain.TransactionAggregate.Action;

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

        public Handler(IOrderHistoryRepository orderHistoryRepository, IOtpTextNowClient otpTextNowClient,
            IRunOtpClient runOtpClient, ITransactionRepository transactionRepository, IScopeContext scopeContext,
            UserManager<AppUser> userManager)
        {
            _orderHistoryRepository = orderHistoryRepository;
            _otpTextNowClient = otpTextNowClient;
            _runOtpClient = runOtpClient;
            _transactionRepository = transactionRepository;
            _scopeContext = scopeContext;
            _userManager = userManager;
        }

        public async Task<IResult> Handle(GetListOrderHistoryQueries request, CancellationToken cancellationToken)
        {
            var queryable = await _orderHistoryRepository
                .FindAll()
                .OrderByDescending(x => x.CreatedDate).ToQueryResultAsync(request.Skip, request.Take);
            var result = new QueryResult<OrderHistoryDto>
            {
                Count = queryable.Count,
                Items = queryable.Items
                    .Select(x => new OrderHistoryDto(
                        x.Id,
                        x.RequestId,
                        x.NumberPhone,
                        x.Message,
                        x.OtpCode,
                        x.WebType,
                        x.Status,
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
                item.RequestId,
                item.NumberPhone,
                item.Message,
                item.OtpCode,
                item.WebType,
                item.Status,
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
                throw new Exception("Người dùng không tồn tại, nhập lại thử apiKey");
            }

            if (user.Balance < 0)
            {
                throw new Exception("Tài khoản của bạn không đủ để sử dụng dịch vụ, xin vui lòng nạp thêm tiền");
            }

            switch (request.WebType)
            {
                case WebType.RunOtp:
                    var resultRunOtpResponse = await _runOtpClient.CreateRequest(_scopeContext.CurrentAccountId);
                    return Results.Ok(ResultModel<CreateOrderResponseClient>.Create(resultRunOtpResponse));
                case WebType.OtpTextNow:
                    var resultNumberResponse = await _otpTextNowClient.CreateRequest(_scopeContext.CurrentAccountId);
                    return Results.Ok(ResultModel<CreateOrderResponseClient>.Create(resultNumberResponse));
            }

            return Results.Ok();
        }
    }
}