using System.Linq.Expressions;
using RunOtp.Domain.WebConfigurationAggregate;
using RunOtp.Driver.OtpTextNow;

namespace RunOtp.WebApi.UseCase.WebConfigurations;

public struct MutateWebConfiguration
{
    public record GetListWebConfigurationQueries(int Skip, int Take, string Query) : IQueries;

    public record GetWebConfigurationQuery : IQuery
    {
        public Guid Id { get; init; }

        internal class GetSpec : SpecificationBase<WebConfiguration>
        {
            private readonly Guid _id;

            public GetSpec(Guid id)
            {
                _id = id;
            }

            public override Expression<Func<WebConfiguration, bool>> Criteria => x => x.Id == _id;

            internal class GetValidator : AbstractValidator<GetWebConfigurationQuery>
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

    public record CreateWebConfigurationCommand : ICreateCommand
    {
        public string ApiSecret { get; set; }
        public string Url { get; set; }
        public string WebName { get; set; }
        public string Endpoint { get; set; }
        public WebType WebType { get; set; }

        public WebConfiguration ToCustomerEntity()
        {
            return new WebConfiguration(ApiSecret, Url, WebName, Endpoint, WebType);
        }

        internal class Validator : AbstractValidator<CreateWebConfigurationCommand>
        {
            public Validator()
            {
                RuleFor(x => x.ApiSecret)
                    .NotNull()
                    .NotEmpty()
                    .WithMessage("Bắt buộc phải nhập ApiSecret");
                RuleFor(x => x.Url)
                    .NotNull()
                    .NotEmpty()
                    .WithMessage("Bắt buộc phải nhập Url");
            }
        }
    }

    public record UpdateWebConfigurationCommand : IUpdateCommand<Guid>
    {
        public Guid Id { get; init; }
        public string ApiSecret { get; set; }
        public string Url { get; set; }
        public string WebName { get; set; }
        public string Endpoint { get; set; }
        public WebStatus Status { get; set; }


        internal class Validator : AbstractValidator<UpdateWebConfigurationCommand>
        {
            public Validator()
            {
                RuleFor(x => x.ApiSecret)
                    .NotNull()
                    .NotEmpty()
                    .WithMessage("Bắt buộc phải nhập ApiSecret");
                RuleFor(x => x.Url)
                    .NotNull()
                    .NotEmpty()
                    .WithMessage("Bắt buộc phải nhập Url");
            }
        }
    }

    internal class Handler : IRequestHandler<GetListWebConfigurationQueries, IResult>,
        IRequestHandler<GetWebConfigurationQuery, IResult>, IRequestHandler<CreateWebConfigurationCommand, IResult>,
        IRequestHandler<UpdateWebConfigurationCommand, IResult>
    {
        private readonly IWebConfigurationRepository _webConfigurationRepository;
        private readonly IOtpTextNowClient _otpTextNowClient;

        public Handler(IWebConfigurationRepository webConfigurationRepository, IOtpTextNowClient otpTextNowClient)
        {
            _webConfigurationRepository = webConfigurationRepository;
            _otpTextNowClient = otpTextNowClient;
        }

        public async Task<IResult> Handle(GetListWebConfigurationQueries request, CancellationToken cancellationToken)
        {
            var queryable = await _webConfigurationRepository
                .FindAll()
                .OrderByDescending(x => x.CreatedDate).ToQueryResultAsync(request.Skip, request.Take);
            var result = new QueryResult<WebConfigurationDto>
            {
                Count = queryable.Count,
                Items = queryable.Items
                    .Select(x => new WebConfigurationDto(x.Id, x.ApiSecret, x.Url, x.WebName, x.Endpoint, x.Status,
                        x.Selected,
                        x.CreatedDate,
                        x.LastUpdatedDate))
                    .ToList()
            };
            return Results.Ok(ResultModel<QueryResult<WebConfigurationDto>>.Create(result));
        }

        public async Task<IResult> Handle(GetWebConfigurationQuery request, CancellationToken cancellationToken)
        {
            var item = await _webConfigurationRepository.GetByIdAsync(request.Id);
            if (item is null) throw new Exception($"Không tìm thấy bản ghi item={request.Id}");
            var result = new WebConfigurationDto(
                item.Id,
                item.ApiSecret,
                item.Url,
                item.WebName,
                item.Endpoint,
                item.Status,
                item.Selected,
                item.CreatedDate,
                item.LastUpdatedDate);
            return Results.Ok(ResultModel<WebConfigurationDto>.Create(result));
        }

        public async Task<IResult> Handle(CreateWebConfigurationCommand request, CancellationToken cancellationToken)
        {
            var webConfiguration = request.ToCustomerEntity();
            _webConfigurationRepository.Add(webConfiguration);
            await _webConfigurationRepository.CommitAsync();
            return Results.Ok();
        }

        public async Task<IResult> Handle(UpdateWebConfigurationCommand request, CancellationToken cancellationToken)
        {
            var item = await _webConfigurationRepository.GetByIdAsync(request.Id);
            if (item is null) throw new Exception($"Không tìm thấy bản ghi id={request.Id}");

            item.Update(request.ApiSecret, request.Url, request.WebName, request.Endpoint, request.Status);
            _webConfigurationRepository.Update(item);
            await _webConfigurationRepository.CommitAsync();
            return Results.Ok();
        }
    }
}