﻿using Microsoft.Extensions.DependencyInjection;
using RunOtp.Driver.OtpTextNow;
using RunOtp.Driver.RentOtp;
using RunOtp.Driver.RunOtp;

namespace RunOtp.Driver;

public static class Extensions
{
    public static IServiceCollection AddClient(this IServiceCollection services)
    {
        services.AddTransient<IOtpTextNowClient, OtpTextNowClient>();
        services.AddTransient<IRunOtpClient, RunOtpClient>();
        services.AddTransient<IRentCodeTextNowClient, RentCodeTextNowClient>();
        return services;
    }
}