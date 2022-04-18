namespace RunOtp.Driver.OtpTextNow;

public interface IOtpTextNowClient
{
    Task<List<ServicesResponse>> GetAllServices();
    Task<CreateOrderResponseClient> CreateRequest(Guid userId);
    Task<OtpCodeResponse> CheckOtpRequest(string requestId);
}