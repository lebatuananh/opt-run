using RunOtp.Domain.OrderHistory;
using RunOtp.Domain.WebConfigurationAggregate;

namespace RunOtp.WebApi.UseCase.OrderHistories;
public record OrderHistoryDto(Guid Id,
    string NumberPhone,
    string Message,
    WebType WebType,
    OrderStatus Status,
    string OtpCode,
    string Username,
    DateTimeOffset CreatedDate,
    DateTimeOffset LastUpdatedDate);