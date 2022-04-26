namespace RunOtp.WebApi.UseCase.ErrorLog;

public record ErrorLogDto(long Id, string Message, string MessageTemplate, string Level, DateTimeOffset TimeStamp,
    string Exception, string LogEvent, string Properties);