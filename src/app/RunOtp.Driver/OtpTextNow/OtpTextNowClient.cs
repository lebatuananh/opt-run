using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using RunOtp.Domain.OrderHistory;
using RunOtp.Domain.UserAggregate;
using RunOtp.Domain.WebConfigurationAggregate;
using RunOtp.Domain.TransactionAggregate;
using Shared.Extensions;
using Shared.HttpClient;
using Shared.SeedWork;
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
        var response = await GetAsync<NumberResponse>(url, ClientConstant.ClientName, ClientConstant.OtpTextNow.Url);
        if (response?.Number is null || response.RequestId is null) return null;
        try
        {
            var entity = new OrderHistory(response.RequestId, response.Number, string.Empty,
                WebType.OtpTextNow, OrderStatus.Created, userId);
            _orderHistoryRepository.Add(entity);
            await _orderHistoryRepository.CommitAsync();
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null)
            {
                throw new Exception("Người dùng không tồn tại");
            }

            var totalAmount = user.Discount > 0 ? user.Discount : AppUser.OtpPrice;
            var entityTransaction = new Transaction(userId, totalAmount,
                "Thanh toán cho dịch vụ code",
                "Account", PaymentGateway.Wallet, Action.Deduction, entity.Id.ToString());
            _transactionRepository.Add(entityTransaction);

            await _transactionRepository.CommitAsync();
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

        var transaction = await _transactionRepository.GetSingleAsync(x => x.Ref == item.Id.ToString());
        if (transaction is null) throw new Exception($"Không tìm thấy bản ghi ref={item.Id}");
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
                            transaction.MarkError();
                            await _transactionRepository.CommitAsync();
                            break;
                        default:
                        {
                            if (response.OtpCode.IsNumeric())
                            {
                                item.Success(response.OtpCode);
                                await UpdateAndSaveDataAsync(item);
                                var user = await _userManager
                                    .FindByIdAsync(item.UserId.ToString());
                                if (user is null)
                                {
                                    throw new Exception("Người dùng không tồn tại");
                                }

                                user.SubtractMoneyOtp();
                                var result = await _userManager.UpdateAsync(user);
                                if (!result.Succeeded)
                                {
                                    throw new Exception("Đã có lỗi xảy ra, xin vui lòng thử lại sau");
                                }

                                transaction.MarkCompleted();
                                await _transactionRepository.CommitAsync();
                            }
                            else
                            {
                                item.Error(response.OtpCode);
                                await UpdateAndSaveDataAsync(item);
                                transaction.MarkError();
                                await _transactionRepository.CommitAsync();
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
        _orderHistoryRepository.Update(item);
        await _orderHistoryRepository.CommitAsync();
    }
}