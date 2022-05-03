using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using RunOtp.Domain.OrderHistory;
using RunOtp.Domain.TransactionAggregate;
using RunOtp.Domain.UserAggregate;
using RunOtp.Domain.WebConfigurationAggregate;
using RunOtp.Driver.OtpTextNow;
using Shared.Extensions;
using Shared.HttpClient;
using Action = RunOtp.Domain.TransactionAggregate.Action;


namespace RunOtp.Driver.RunOtp;

public class RunOtpClient : BaseApiClient, IRunOtpClient
{
    private readonly IWebConfigurationRepository _webConfigurationRepository;
    private readonly IOrderHistoryRepository _orderHistoryRepository;
    private readonly UserManager<AppUser> _userManager;
    private readonly ITransactionRepository _transactionRepository;


    public RunOtpClient(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor,
        IWebConfigurationRepository webConfigurationRepository, IOrderHistoryRepository orderHistoryRepository,
        UserManager<AppUser> userManager, ITransactionRepository transactionRepository) : base(
        httpClientFactory, httpContextAccessor)
    {
        _webConfigurationRepository = webConfigurationRepository;
        _orderHistoryRepository = orderHistoryRepository;
        _userManager = userManager;
        _transactionRepository = transactionRepository;
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
            string.IsNullOrEmpty(data?.RequestId))
        {
            throw new Exception("Temporarily out of phone number, please try again later");
        }

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
            throw new Exception(e.Message);
        }
    }

    public async Task<OtpCodeResponse> CheckOtpRequest(OrderHistory orderHistory)
    {
        var url =
            $"{ClientConstant.RunOtp.Endpoint}/api.php?apikey={ClientConstant.RunOtp.ApiKey}&action=data-request&requestId={orderHistory.RequestId}";
        var response =
            await GetAsync<RunOtpResponse>(
                url,
                ClientConstant.ClientName,
                ClientConstant.RunOtp.Url);
        if (response is null || response.Results is null)
        {
            throw new Exception("An error occurred, please try again later");
        }

        var data = response.Results.Data?.First();

        if (data is null)
        {
            throw new Exception("An error occurred, please try again later");
        }

        var responseClient = new OtpCodeResponse();

        try
        {
            if (data.Status.Equals(ClientConstant.RunOtp.Processing))
            {
                responseClient.Status = OrderStatus.Processing;
                orderHistory.Processing(ClientConstant.RunOtp.ProcessingMessage);
                await _orderHistoryRepository.CommitAsync();
            }
            else if (data.Status.Equals(ClientConstant.RunOtp.Timeout))
            {
                responseClient.Status = OrderStatus.Processing;
                orderHistory.Processing(ClientConstant.RunOtp.TimeoutMessage);
                await _orderHistoryRepository.CommitAsync();
            }
            else if (data.Status.Equals(ClientConstant.RunOtp.Cancel))
            {
                responseClient.Status = OrderStatus.Error;
                orderHistory.Processing(ClientConstant.RunOtp.CancelMessage);
                await _orderHistoryRepository.CommitAsync();
            }
            else if (data.Status.Equals(ClientConstant.RunOtp.Success) && !string.IsNullOrEmpty(data.Otp))
            {
                if (data.Otp.IsNumeric())
                {
                    responseClient.OtpCode = data.Otp;
                    responseClient.Status = OrderStatus.Success;
                    orderHistory.Success(data.Otp);
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
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }

        return responseClient;
    }
}