using RunOtp.Domain.OrderHistory;
using RunOtp.Driver.OtpTextNow;

namespace RunOtp.Driver.RunOtp;

public interface IRunOtpClient
{
    Task<CreateOrderResponseClient> CreateRequest(Guid userId);
    Task<OtpCodeResponse> CheckOtpRequest(OrderHistory orderHistory);
}