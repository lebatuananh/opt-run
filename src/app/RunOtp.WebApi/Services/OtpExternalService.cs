﻿using Microsoft.AspNetCore.Identity;
using RunOtp.Domain.UserAggregate;
using RunOtp.Domain.WebConfigurationAggregate;
using RunOtp.Driver;
using RunOtp.Driver.OtpTextNow;
using RunOtp.Driver.RentOtp;
using RunOtp.Driver.RunOtp;

namespace RunOtp.WebApi.Services;

public class OtpExternalService : IOtpExternalService
{
    private readonly IRunOtpClient _runOtpClient;
    private readonly IOtpTextNowClient _otpTextNowClient;
    private readonly IRentCodeTextNowClient _rentCodeTextNowClient;
    private readonly UserManager<AppUser> _userManager;

    public OtpExternalService(IRunOtpClient runOtpClient, IOtpTextNowClient otpTextNowClient,
        UserManager<AppUser> userManager, IRentCodeTextNowClient rentCodeTextNowClient)
    {
        _runOtpClient = runOtpClient;
        _otpTextNowClient = otpTextNowClient;
        _userManager = userManager;
        _rentCodeTextNowClient = rentCodeTextNowClient;
    }

    public async Task<CreateOrderResponseClient> CreateOtpRequest(string apiKey, WebType webType)
    {
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new Exception("Apikey không tồn tại");
        }

        var user = await _userManager.Users.SingleOrDefaultAsync(x => x.ClientSecret == apiKey);
        if (user is null)
        {
            throw new Exception("User does not exist, try re-entering apiKey");
        }

        if (user.Balance < 0)
        {
            throw new Exception("Tài khoản của bạn không đủ để sử dụng dịch vụ, xin vui lòng nạp thêm tiền");
        }

        switch (webType)
        {
            case WebType.RunOtp:
                // var resultRunOtpResponse = await _runOtpClient.CreateRequest(user.Id);
                return null;
            case WebType.OtpTextNow:
                var resultNumberResponse = await _otpTextNowClient.CreateRequest(user.Id);
                return resultNumberResponse;
            case WebType.RentOtp:
                var resultNumberRentOtpResponse = await _rentCodeTextNowClient.CreateRequest(user.Id);
                return resultNumberRentOtpResponse;
            default:
                throw new Exception("Brand new, you enter the correct site to get the code");
        }
    }

    public async Task<OtpExternalResponse> CheckOtpRequest(string apiKey, WebType webType, Guid requestId)
    {
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new Exception("Apikey không tồn tại");
        }

        var user = await _userManager.Users.SingleOrDefaultAsync(x => x.ClientSecret == apiKey);
        if (user is null)
        {
            throw new Exception("User does not exist, try re-entering apiKey");
        }

        switch (webType)
        {
            case WebType.RunOtp:
                // var resultRunOtpResponse = await _runOtpClient.CheckRequest(user.Id, requestId.ToString());
                return null;
            case WebType.OtpTextNow:
                var resultNumberResponse = await _otpTextNowClient.CheckOtpRequest(requestId.ToString());
                return new OtpExternalResponse()
                {
                    Message = resultNumberResponse.Message,
                    OtpCode = resultNumberResponse.OtpCode,
                    Status = resultNumberResponse.Status
                };
            case WebType.RentOtp:
                var resultNumberRentResponse = await _rentCodeTextNowClient.CheckOtpRequest(requestId.ToString());
                return new OtpExternalResponse()
                {
                    Message = resultNumberRentResponse.Message,
                    OtpCode = resultNumberRentResponse.OtpCode,
                    Status = resultNumberRentResponse.Status
                };
            default:
                throw new Exception("Xịn mới bạn nhập đúng site lấy lấy mã code");
        }
    }
}