using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RunOtp.Domain.ErrorLog;
using Shared.Constants;
using Shared.Logging.LogError;
using Shared.Repositories;

namespace RunOtp.Infrastructure.Repositories;

public class ErrorLogRepository : IErrorLogRepository
{
    private readonly MainDbContext _mainDbContext;

    public ErrorLogRepository(MainDbContext mainDbContext)
    {
        _mainDbContext = mainDbContext;
    }


    public bool AutoSaveChanges { get; set; } = true;

    protected virtual async Task<int> AutoSaveChangesAsync()
    {
        return AutoSaveChanges ? await _mainDbContext.SaveChangesAsync() : (int)SavedStatus.WillBeSavedExplicitly;
    }

    public async Task<QueryResult<Log>> GetLogsAsync(string? query, int skip = 0, int take = 10)
    {
        QueryResult<Log> queryable;
        if (!string.IsNullOrEmpty(query))
        {
            Expression<Func<Log, bool>> searchCondition = x =>
                x.LogEvent.Contains(query) || x.Message.Contains(query) ||
                x.Exception != null && x.Exception.Contains(query);
            queryable = await _mainDbContext.Logs.Where(searchCondition)
                .OrderByDescending(x => x.TimeStamp).ToQueryResultAsync(skip, take);
            return queryable;
        }

        queryable = await _mainDbContext.Logs
            .OrderByDescending(x => x.TimeStamp).ToQueryResultAsync(skip, take);
        return queryable;
    }

    public async Task DeleteLogsOlderThanAsync(DateTime deleteOlderThan)
    {
        var logsToDelete = await _mainDbContext.Logs.Where(x => x.TimeStamp < deleteOlderThan.Date).ToListAsync();

        if (logsToDelete.Count == 0) return;

        _mainDbContext.Logs.RemoveRange(logsToDelete);

        await AutoSaveChangesAsync();
    }
}