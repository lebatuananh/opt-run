using RunOtp.Domain.WebConfigurationAggregate;
using RunOtp.Driver;

namespace RunOtp.WebApi.Services;

public interface IOtpExternalService
{
    public Task<CreateOrderResponseClient> CreateOtpRequest(string apiKey, WebType webType);
    public Task<OtpExternalResponse> CheckOtpRequest(string apiKey, WebType webType, Guid requestId);
}