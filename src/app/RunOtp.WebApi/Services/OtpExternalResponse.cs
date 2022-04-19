using RunOtp.Domain.OrderHistory;

namespace RunOtp.WebApi.Services;

public class OtpExternalResponse
{
    public string OtpCode { get; set; }
    public string Message { get; set; }
    public OrderStatus Status { get; set; }
}