using Microsoft.Extensions.DependencyInjection;
using RunOtp.Driver.OtpTextNow;

namespace RunOtp.Driver;

public static class Extensions
{
    public static IServiceCollection AddClient(this IServiceCollection services)
    {
        services.AddTransient<IOtpTextNowClient, OtpTextNowClient>();
        return services;
    }
}