using RunOtp.Domain.ErrorLog;

namespace RunOtp.WebApi.UseCase.ErrorLog;

public struct MutateErrorLog
{
    public record GetListErrorLogQueries(int Skip, int Take, string Query) : IQueries;

    public record DeleteLogsOlderThanCommand(DateTime DeleteOlderThan) : ICommand;

    internal class Handler : IRequestHandler<GetListErrorLogQueries, IResult>,
        IRequestHandler<DeleteLogsOlderThanCommand, IResult>
    {
        private readonly IErrorLogRepository _errorLogRepository;

        public Handler(IErrorLogRepository errorLogRepository)
        {
            _errorLogRepository = errorLogRepository;
        }

        public async Task<IResult> Handle(GetListErrorLogQueries request, CancellationToken cancellationToken)
        {
            var queryResult = await _errorLogRepository.GetLogsAsync(request.Query, request.Skip, request.Take);
            var result = new QueryResult<ErrorLogDto>
            {
                Count = queryResult.Count,
                Items = queryResult.Items
                    .Select(x => new ErrorLogDto(
                        x.Id,
                        x.Message,
                        x.MessageTemplate,
                        x.Level,
                        x.TimeStamp,
                        x.Exception,
                        x.LogEvent,
                        x.Properties))
                    .ToList()
            };
            return Results.Ok(ResultModel<QueryResult<ErrorLogDto>>.Create(result));
        }

        public async Task<IResult> Handle(DeleteLogsOlderThanCommand request, CancellationToken cancellationToken)
        {
            await _errorLogRepository.DeleteLogsOlderThanAsync(request.DeleteOlderThan);
            return Results.Ok();
        }
    }
}