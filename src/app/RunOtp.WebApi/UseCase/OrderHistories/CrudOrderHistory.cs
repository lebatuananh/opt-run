using System.Linq.Expressions;
using RunOtp.Domain.OrderHistory;
using RunOtp.Driver.OtpTextNow;
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

    public record CreateOrderHistoryCommand : ICreateCommand;

    internal class Handler : IRequestHandler<GetListOrderHistoryQueries, IResult>,
        IRequestHandler<GetOrderHistoryQuery, IResult>, IRequestHandler<CreateOrderHistoryCommand, IResult>
    {
        private readonly IOrderHistoryRepository _orderHistoryRepository;
        private readonly IOtpTextNowClient _otpTextNowClient;

        public Handler(IOrderHistoryRepository orderHistoryRepository, IOtpTextNowClient otpTextNowClient)
        {
            _orderHistoryRepository = orderHistoryRepository;
            _otpTextNowClient = otpTextNowClient;
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
            var result = await _otpTextNowClient.CreateRequest();
            return Results.Ok(ResultModel<NumberResponse>.Create(result));
        }
    }
}