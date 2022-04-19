using RunOtp.Domain.OrderHistory;
using RunOtp.Driver.OtpTextNow;

namespace RunOtp.WebApi.Tasks;

public class OtpTextNowTask : IOtpTextNowTask
{
    private readonly IOrderHistoryRepository _orderHistoryRepository;
    private readonly IOtpTextNowClient _otpTextNowClient;

    public OtpTextNowTask(IOrderHistoryRepository orderHistoryRepository, IOtpTextNowClient otpTextNowClient)
    {
        _orderHistoryRepository = orderHistoryRepository;
        _otpTextNowClient = otpTextNowClient;
    }

    public async Task ExecuteAsync()
    {
        var orderHistories =
            await _orderHistoryRepository.FindAll(x =>
                    x.Status != OrderStatus.Error && x.Status != OrderStatus.Success).Take(1000)
                .OrderBy(x => x.CreatedDate)
                .ToListAsync();
        var orderRequestIds = orderHistories.Select(x => x.Id);
        foreach (var item in orderRequestIds)
        {
            await _otpTextNowClient.CheckOtpRequest(item.ToString());
        }
    }
}