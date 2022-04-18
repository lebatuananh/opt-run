using RunOtp.Domain.OrderHistory;
using RunOtp.Domain.WebConfigurationAggregate;

namespace RunOtp.WebApi.UseCase.OrderHistories;

public record OrderHistoryDto(Guid Id, string RequestId, string NumberPhone, string Message, string OtpCode,
    WebType WebType,
    OrderStatus Status, DateTimeOffset CreatedDate, DateTimeOffset LastUpdatedDate);

