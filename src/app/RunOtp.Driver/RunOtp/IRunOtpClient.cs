namespace RunOtp.Driver.RunOtp;

public interface IRunOtpClient
{
    Task<CreateOrderResponseClient> CreateRequest(Guid userId);
    Task<RunOtpResponse> CheckRequest(Guid userId,string requestId);
}