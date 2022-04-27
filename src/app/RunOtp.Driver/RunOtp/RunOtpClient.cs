using IdentityModel;
using Microsoft.AspNetCore.Http;
using RunOtp.Domain.OrderHistory;
using RunOtp.Domain.WebConfigurationAggregate;
using Shared.Extensions;
using Shared.HttpClient;
using Shared.SeedWork;

namespace RunOtp.Driver.RunOtp;

public class RunOtpClient : BaseApiClient, IRunOtpClient
{
    private readonly IWebConfigurationRepository _webConfigurationRepository;
    private readonly IOrderHistoryRepository _orderHistoryRepository;

    public RunOtpClient(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor,
        IWebConfigurationRepository webConfigurationRepository, IOrderHistoryRepository orderHistoryRepository) : base(
        httpClientFactory, httpContextAccessor)
    {
        _webConfigurationRepository = webConfigurationRepository;
        _orderHistoryRepository = orderHistoryRepository;
    }


    public async Task<CreateOrderResponseClient> CreateRequest(Guid userId)
    {
        var url =
            $"{ClientConstant.RunOtp.Endpoint}?apikey={ClientConstant.RunOtp.ApiKey}&action=create-request&serviceId=1&count=1";
        var response =
            await GetAsync<RunOtpResponse>(
                url,
                ClientConstant.ClientName,
                ClientConstant.RunOtp.Url);
        var data = response.Results.Data?.First();
        if (data is null || string.IsNullOrEmpty(data.Phone) ||
            string.IsNullOrEmpty(data?.RequestId)) return null;
        try
        {
            var entity = new OrderHistory(
                data.RequestId,
                data.Phone,
                string.Empty,
                WebType.RunOtp,
                OrderStatus.Created,
                userId,
                data.CreateTime.UnixSecondsToDateTime(true),
                data.Otp);
            _orderHistoryRepository.Add(entity);
            await _orderHistoryRepository.CommitAsync();
            return new CreateOrderResponseClient() { RequestId = entity.Id, PhoneNumber = data.Phone };
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    public async Task<RunOtpResponse> CheckOtpRequest(OrderHistory orderHistory)
    {
        var url =
            $"{ClientConstant.RunOtp.Endpoint}/api.php?apikey={ClientConstant.RunOtp.ApiKey}&action=data-request&requestId={orderHistory.RequestId}";
        var response =
            await GetAsync<RunOtpResponse>(
                url,
                ClientConstant.ClientName,
                ClientConstant.RunOtp.Url);
        try
        {
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return null;
    }
}