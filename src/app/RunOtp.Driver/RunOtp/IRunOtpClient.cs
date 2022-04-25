using RunOtp.Domain.OrderHistory;

namespace RunOtp.Driver.RunOtp;

public interface IRunOtpClient
{
    Task<CreateOrderResponseClient> CreateRequest(Guid userId);
    Task<RunOtpResponse> CheckOtpRequest(OrderHistory orderHistory);
}