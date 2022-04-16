namespace RunOtp.Driver.OtpTextNow;

public interface IOtpTextNowClient
{
    Task<List<ServicesResponse>> GetAllServices();
    Task<NumberResponse> CreateRequest();
    Task<OtpCodeResponse> CheckOtpRequest(string requestId);
}