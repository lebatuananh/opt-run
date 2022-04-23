using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using RunOtp.Domain.OrderHistory;
using RunOtp.Domain.TransactionAggregate;
using RunOtp.Domain.UserAggregate;
using RunOtp.Domain.WebConfigurationAggregate;
using RunOtp.Driver.OtpTextNow;
using Serilog;
using Shared.Extensions;
using Shared.HttpClient;
using Action = RunOtp.Domain.TransactionAggregate.Action;

namespace RunOtp.Driver.RentOtp;

public class RentCodeTextNowClient : BaseApiClient, IRentCodeTextNowClient
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IOrderHistoryRepository _orderHistoryRepository;
    private readonly ITransactionRepository _transactionRepository;

    public RentCodeTextNowClient(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager, IOrderHistoryRepository orderHistoryRepository,
        ITransactionRepository transactionRepository) : base(httpClientFactory, httpContextAccessor)
    {
        _userManager = userManager;
        _orderHistoryRepository = orderHistoryRepository;
        _transactionRepository = transactionRepository;
    }

    public async Task<CreateOrderResponseClient> CreateRequest(Guid userId)
    {
        var url =
            $"{ClientConstant.RentOtp.Endpoint}/get-phone/?access_token={ClientConstant.RentOtp.ApiKey}";
        var response =
            await GetObjectAsync<RentCodeNumberResponse>(url, ClientConstant.ClientName, ClientConstant.RentOtp.Url);

        if (string.IsNullOrEmpty(response.Phone))
        {
            for (var i = 0; i < 5; i++)
            {
                response = await GetObjectAsync<RentCodeNumberResponse>(url, ClientConstant.ClientName,
                    ClientConstant.RentOtp.Url);
                if (!string.IsNullOrEmpty(response.Phone))
                {
                    break;
                }

                await Task.Delay(300);
            }
        }

        Log.Error("Request: ${RequestId} - Balance: ${Balance}", response.Phone, response.Balance);

        if (string.IsNullOrEmpty(response.Phone)) throw new Exception("Can't get phone number");
        try
        {
            var entity = new OrderHistory(string.Empty, response.Phone, string.Empty,
                WebType.RentOtp, OrderStatus.Created, userId);
            _orderHistoryRepository.Add(entity);
            await _orderHistoryRepository.CommitAsync();
            return new CreateOrderResponseClient { RequestId = entity.Id, PhoneNumber = response.Phone };
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
            $"{ClientConstant.RentOtp.Endpoint}/get-otp/?access_token={ClientConstant.RentOtp.ApiKey}&phone={item.NumberPhone}";
        var response =
            await GetObjectAsync<RentCodeNumberResponse>(url, ClientConstant.ClientName, ClientConstant.RentOtp.Url);
        var responseClient = new OtpCodeResponse()
        {
            Message = response.Message,
        };
        try
        {
            if (!string.IsNullOrEmpty(response.Otp))
            {
                if (response.Otp.IsNumeric())
                {
                    responseClient.Status = OrderStatus.Success;
                    item.Success(response.Otp);
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
            }
            else
                switch (response.Status)
                {
                    case RentCodeNumberResponse.StatusProcessing:
                        responseClient.Status = OrderStatus.Processing;
                        item.Processing(RentCodeNumberResponse.StatusProcessing);
                        await _transactionRepository.CommitAsync();
                        break;
                    case RentCodeNumberResponse.StatusError:
                        responseClient.Status = OrderStatus.Error;
                        item.Error(string.Empty);
                        await _transactionRepository.CommitAsync();
                        break;
                }
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }

        return responseClient;
    }
}