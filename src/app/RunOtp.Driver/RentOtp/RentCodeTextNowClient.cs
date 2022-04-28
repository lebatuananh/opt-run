using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
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
        try
        {
            var url =
                $"{ClientConstant.RentOtp.Endpoint}/get-phone/?access_token={ClientConstant.RentOtp.ApiKey}";
            var response =
                await GetObjectAsync<RentCodeNumberResponse>(url, ClientConstant.ClientName,
                    ClientConstant.RentOtp.Url);

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

            Log.Error("Request create: {RequestId} - Balance: {Balance} - UserId: {UserId}", response.Phone,
                response.Balance, userId);

            if (string.IsNullOrEmpty(response.Phone)) throw new Exception("Can't get phone number");

            var entity = new OrderHistory(string.Empty, response.Phone, string.Empty,
                WebType.RentOtp, OrderStatus.Created, userId);
            _orderHistoryRepository.Add(entity);
            await _orderHistoryRepository.CommitAsync();
            return new CreateOrderResponseClient { RequestId = entity.Id, PhoneNumber = response.Phone };
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task<OtpCodeResponse> CheckOtpRequest(OrderHistory orderHistory)
    {
        var url =
            $"{ClientConstant.RentOtp.Endpoint}/get-otp/?access_token={ClientConstant.RentOtp.ApiKey}&phone={orderHistory.NumberPhone}";
        var response =
            await GetObjectAsync<RentCodeNumberResponse>(url, ClientConstant.ClientName, ClientConstant.RentOtp.Url);
        // Log.Error("Response RentCode: {Response} - {UserId}", JsonConvert.SerializeObject(response),
        //     orderHistory.UserId);
        var responseClient = new OtpCodeResponse()
        {
            Message = response.Message,
        };
        try
        {
            if (!string.IsNullOrEmpty(response.Otp) && response.Status == RentCodeNumberResponse.StatusSuccess)
            {
                if (response.Otp.IsNumeric())
                {
                    responseClient.Status = OrderStatus.Success;
                    responseClient.OtpCode = response.Otp;
                    orderHistory.Success(response.Otp);
                    _orderHistoryRepository.Update(orderHistory);
                    await _orderHistoryRepository.CommitAsync();
                    var user = await _userManager
                        .FindByIdAsync(orderHistory.UserId.ToString());
                    if (user is null)
                    {
                        throw new Exception("User not found");
                    }

                    var transaction =
                        await _transactionRepository.GetSingleAsync(x => x.Ref == orderHistory.Id.ToString());
                    if (transaction == null)
                    {
                        var totalAmount = user.Discount > 0 ? user.Discount : AppUser.OtpPrice;
                        var entityTransaction = new Transaction(user.Id, totalAmount,
                            $"Thanh toán cho dịch vụ code : {orderHistory.OtpCode} - {orderHistory.Id}",
                            "Account", PaymentGateway.Wallet, Action.Deduction, orderHistory.Id.ToString());
                        _transactionRepository.Add(entityTransaction);
                        await _transactionRepository.CommitAsync();
                    }
                }
            }
            else if (!response.Success && response.Description == RentCodeNumberResponse.NoPhoneMessage)
            {
                responseClient.Status = OrderStatus.Error;
                orderHistory.Error(RentCodeNumberResponse.StatusError);
                await _orderHistoryRepository.CommitAsync();
            }
            else
            {
                switch (response.Status)
                {
                    case RentCodeNumberResponse.StatusProcessing:
                        responseClient.Status = OrderStatus.Processing;
                        orderHistory.Processing(RentCodeNumberResponse.StatusProcessing);
                        await _orderHistoryRepository.CommitAsync();
                        break;
                    case RentCodeNumberResponse.StatusError:
                        responseClient.Status = OrderStatus.Error;
                        orderHistory.Error(RentCodeNumberResponse.StatusError);
                        await _orderHistoryRepository.CommitAsync();
                        break;
                }
            }
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }

        return responseClient;
    }
}