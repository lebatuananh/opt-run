using RunOtp.Domain.WebConfigurationAggregate;

namespace RunOtp.WebApi.UseCase.WebConfigurations;

public struct SelectWebConfiguration
{
    public record ChangeSelectedCommand(Guid WebId) : ICommand;


    internal class Handler : IRequestHandler<ChangeSelectedCommand, IResult>
    {
        private readonly IWebConfigurationRepository _webConfigurationRepository;

        public Handler(IWebConfigurationRepository webConfigurationRepository)
        {
            _webConfigurationRepository = webConfigurationRepository;
        }

        public async Task<IResult> Handle(ChangeSelectedCommand request, CancellationToken cancellationToken)
        {
            var webConfigQuery = await _webConfigurationRepository.GetByIdAsync(request.WebId);
            if (webConfigQuery is null)
            {
                throw new Exception($"WebConfig not found with Id={request.WebId}");
            }

            var listWebConfig = _webConfigurationRepository.FindAll().ToList();
            foreach (var item in listWebConfig)
            {
                if (item.Id == request.WebId)
                {
                    item.ChangeSelected();
                }
                else
                {
                    item.ChangeUnSelected();
                }

                _webConfigurationRepository.Update(item);
            }

            await _webConfigurationRepository.CommitAsync();
            return Results.Ok();
        }
    }
}