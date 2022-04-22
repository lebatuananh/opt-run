using RunOtp.Driver.OtpTextNow;

namespace RunOtp.Driver.RentOtp;

public interface IRentCodeTextNowClient
{
    Task<CreateOrderResponseClient> CreateRequest(Guid userId);
    Task<OtpCodeResponse> CheckOtpRequest(string requestId);
}