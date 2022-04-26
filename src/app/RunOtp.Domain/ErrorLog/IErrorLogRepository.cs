using Shared.Logging.LogError;
using Shared.Repositories;

namespace RunOtp.Domain.ErrorLog;

public interface IErrorLogRepository
{
    bool AutoSaveChanges { get; set; }
    Task<QueryResult<Log>> GetLogsAsync(string? query, int skip = 1, int take = 10);

    Task DeleteLogsOlderThanAsync(DateTime deleteOlderThan);
}