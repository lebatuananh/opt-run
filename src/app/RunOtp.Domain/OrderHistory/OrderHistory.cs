using RunOtp.Domain.UserAggregate;
using RunOtp.Domain.WebConfigurationAggregate;
using Shared.SeedWork;

namespace RunOtp.Domain.OrderHistory;

public class OrderHistory : ModifierTrackingEntity, IAggregateRoot
{
    public string RequestId { get; set; }
    public string NumberPhone { get; set; }
    public string Message { get; set; }
    public WebType WebType { get; set; }
    public OrderStatus Status { get; set; }
    public string OtpCode { get; set; }
    public Guid UserId { get; set; }
    public virtual AppUser AppUser { get; set; }

    public OrderHistory(string requestId, string numberPhone, string message, WebType webType,
        OrderStatus status, Guid userId)
    {
        RequestId = requestId;
        NumberPhone = numberPhone;
        Message = message;
        WebType = webType;
        Status = status;
        UserId = userId;
    }

    public void Error(string otpCode)
    {
        OtpCode = otpCode;
        Status = OrderStatus.Error;
    }

    public void Success(string otpCode)
    {
        OtpCode = otpCode;
        Status = OrderStatus.Success;
    }

    public void Processing(string otpCode)
    {
        OtpCode = otpCode;
        Status = OrderStatus.Processing;
    }
}