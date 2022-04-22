using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using RunOtp.Domain.OrderHistory;
using RunOtp.Domain.TransactionAggregate;
using RunOtp.Domain.UserAggregate;
using RunOtp.Domain.WebConfigurationAggregate;
using Serilog;
using Shared.Extensions;
using Shared.HttpClient;
using Action = RunOtp.Domain.TransactionAggregate.Action;

namespace RunOtp.Driver.OtpTextNow;

public class OtpTextNowClient : BaseApiClient, IOtpTextNowClient
{
    private readonly IWebConfigurationRepository _webConfigurationRepository;
    private readonly UserManager<AppUser> _userManager;
    private readonly IOrderHistoryRepository _orderHistoryRepository;
    private readonly ITransactionRepository _transactionRepository;

    public OtpTextNowClient(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor,
        IWebConfigurationRepository webConfigurationRepository, IOrderHistoryRepository orderHistoryRepository,
        UserManager<AppUser> userManager,
        ITransactionRepository transactionRepository) : base(
        httpClientFactory, httpContextAccessor)
    {
        _webConfigurationRepository = webConfigurationRepository;
        _orderHistoryRepository = orderHistoryRepository;
        _userManager = userManager;
        _transactionRepository = transactionRepository;
    }

    public async Task<List<ServicesResponse>> GetAllServices()
    {
        var url =
            $"{ClientConstant.OtpTextNow.Endpoint}/?key={ClientConstant.OtpTextNow.ApiKey}&action=get_all_services";
        var response =
            await GetListAsync<ServicesResponse>(url, ClientConstant.ClientName, ClientConstant.OtpTextNow.Url);
        return response;
    }

    public async Task<CreateOrderResponseClient> CreateRequest(Guid userId)
    {
        var url =
            $"{ClientConstant.OtpTextNow.Endpoint}/?key={ClientConstant.OtpTextNow.ApiKey}&action=get_number&id=1";
        var response = await GetObjectAsync<NumberResponse>(url, ClientConstant.ClientName, ClientConstant.OtpTextNow.Url);
        if (response.Status == 1)
        {
            throw new Exception("Temporarily out of phone");
        }
        if (response.Number is null || response.RequestId is null)
        {
            for (var i = 0; i < 5; i++)
            {
                response = await GetObjectAsync<NumberResponse>(url, ClientConstant.ClientName,
                    ClientConstant.OtpTextNow.Url);
                if (response.Number is not null && response.RequestId is not null)
                {
                    break;
                }

                await Task.Delay(300);
            }
        }
        Log.Error("Request: ${RequestId} - Number: ${Number}", response.Number, response.RequestId);

        if (response.Number is null || response.RequestId is null) throw new Exception("Can't get phone number");
        try
        {
            var entity = new OrderHistory(response.RequestId, response.Number, string.Empty,
                WebType.OtpTextNow, OrderStatus.Created, userId);
            _orderHistoryRepository.Add(entity);
            await _orderHistoryRepository.CommitAsync();
            return new CreateOrderResponseClient { RequestId = entity.Id, PhoneNumber = response.Number };
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    public async Task<OtpCodeResponse> CheckOtpRequest(string id)
    {
        var item = await _orderHistoryRepository.GetByIdAsync(new Guid(id));
        if (item is null) throw new Exception($"Không tìm thấy bản ghi Id={id}");
        var url =
            $"{ClientConstant.OtpTextNow.Endpoint}/?key={ClientConstant.OtpTextNow.ApiKey}&action=get_code&id={item.RequestId}";
        var response =
            await GetAsync<OtpCodeResponse>(url, ClientConstant.ClientName, ClientConstant.OtpTextNow.Url);
        try
        {
            if (response is not null)
            {
                if (response.Message == "Please send request slow down!" && string.IsNullOrEmpty(response.OtpCode))
                {
                    response.Status = OrderStatus.Processing;
                    item.Processing(string.Empty);
                    await UpdateAndSaveDataAsync(item);
                }
                else
                    switch (response.OtpCode)
                    {
                        case "is_comming":
                            response.Status = OrderStatus.Processing;
                            item.Processing(response.OtpCode);
                            await UpdateAndSaveDataAsync(item);
                            break;
                        case "timeout":
                            response.Status = OrderStatus.Error;
                            item.Error(response.OtpCode);
                            await UpdateAndSaveDataAsync(item);
                            break;
                        default:
                        {
                            if (response.OtpCode.IsNumeric())
                            {
                                response.Status = OrderStatus.Success;
                                item.Success(response.OtpCode);
                                _orderHistoryRepository.Update(item);
                                var user = await _userManager
                                    .FindByIdAsync(item.UserId.ToString());
                                if (user is null)
                                {
                                    throw new Exception("User not found");
                                }

                                var transaction =
                                    await _transactionRepository.GetSingleAsync(x => x.Ref == item.Id.ToString());
                                if (transaction == null)
                                {
                                    var totalAmount = user.Discount > 0 ? user.Discount : AppUser.OtpPrice;
                                    var entityTransaction = new Transaction(user.Id, totalAmount,
                                        $"Thanh toán cho dịch vụ code : {item.OtpCode} - {item.Id}",
                                        "Account", PaymentGateway.Wallet, Action.Deduction, item.Id.ToString());
                                    _transactionRepository.Add(entityTransaction);
                                    await _transactionRepository.CommitAsync();
                                }
                            }
                            else
                            {
                                response.Status = OrderStatus.Error;
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

    private async Task UpdateAndSaveDataAsync(OrderHistory item)
    {
        await _orderHistoryRepository.CommitAsync();
    }
}