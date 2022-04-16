using Microsoft.AspNetCore.Http;
using RunOtp.Domain.OrderHistory;
using RunOtp.Domain.WebConfigurationAggregate;
using Shared.Extensions;
using Shared.HttpClient;
using Shared.SeedWork;

namespace RunOtp.Driver.OtpTextNow;

public class OtpTextNowClient : BaseApiClient, IOtpTextNowClient
{
    private readonly IWebConfigurationRepository _webConfigurationRepository;
    private readonly IOrderHistoryRepository _orderHistoryRepository;
    private readonly IScopeContext _scopeContext;

    public OtpTextNowClient(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor,
        IWebConfigurationRepository webConfigurationRepository, IOrderHistoryRepository orderHistoryRepository,
        IScopeContext scopeContext) : base(
        httpClientFactory, httpContextAccessor)
    {
        _webConfigurationRepository = webConfigurationRepository;
        _orderHistoryRepository = orderHistoryRepository;
        _scopeContext = scopeContext;
    }

    public async Task<List<ServicesResponse>> GetAllServices()
    {
        var url =
            $"{ClientConstant.OtpTextNow.Endpoint}/?key={ClientConstant.OtpTextNow.ApiKey}&action=get_all_services";
        var response =
            await GetListAsync<ServicesResponse>(url, ClientConstant.ClientName, ClientConstant.OtpTextNow.Url);
        return response;
    }

    public async Task<NumberResponse> CreateRequest()
    {
        var url =
            $"{ClientConstant.OtpTextNow.Endpoint}/?key={ClientConstant.OtpTextNow.ApiKey}&action=get_number&id=1";
        var response = await GetAsync<NumberResponse>(url, ClientConstant.ClientName, ClientConstant.OtpTextNow.Url);
        if (response?.Number is null || response.RequestId is null) return null;
        try
        {
            _orderHistoryRepository.Add(new OrderHistory(response.RequestId, response.Number, string.Empty,
                WebType.OtpTextNow, OrderStatus.Created, _scopeContext.CurrentAccountId));
            await _orderHistoryRepository.CommitAsync();
        }
        catch (Exception e)
        {
            throw e;
        }

        return response;
    }

    public async Task<OtpCodeResponse> CheckOtpRequest(string requestId)
    {
        var url =
            $"{ClientConstant.OtpTextNow.Endpoint}/?key={ClientConstant.OtpTextNow.ApiKey}&action=get_code&id={requestId}";
        var item = await _orderHistoryRepository.GetSingleAsync(x => x.RequestId.Equals(requestId));
        if (item is not null)
        {
            var response =
                await GetAsync<OtpCodeResponse>(url, ClientConstant.ClientName, ClientConstant.OtpTextNow.Url);
            try
            {
                if (response is not null)
                {
                    if (response.Message == "Please send request slow down!" && string.IsNullOrEmpty(response.OtpCode))
                    {
                        item.Processing(string.Empty);
                        await UpdateAndSaveDataAsync(item);
                    }
                    else
                        switch (response.OtpCode)
                        {
                            case "is_comming":
                                item.Processing(response.OtpCode);
                                await UpdateAndSaveDataAsync(item);
                                break;
                            case "timeout":
                                item.Error(response.OtpCode);
                                await UpdateAndSaveDataAsync(item);
                                break;
                            default:
                            {
                                if (response.OtpCode.IsNumeric())
                                {
                                    item.Success(response.OtpCode);
                                    await UpdateAndSaveDataAsync(item);
                                }
                                else
                                {
                                    item.Error(response.OtpCode);
                                    await UpdateAndSaveDataAsync(item);
                                }

                                break;
                            }
                        }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return response;
        }

        throw new Exception($"Không tìm thấy bản ghi requestId={requestId}");
    }

    private async Task UpdateAndSaveDataAsync(OrderHistory item)
    {
        _orderHistoryRepository.Update(item);
        await _orderHistoryRepository.CommitAsync();
    }
}