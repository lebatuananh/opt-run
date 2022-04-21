using Microsoft.AspNetCore.Identity;
using RunOtp.Domain.OrderHistory;
using RunOtp.Domain.TransactionAggregate;
using RunOtp.Domain.UserAggregate;
using RunOtp.Driver.OtpTextNow;

namespace RunOtp.WebApi.Tasks;

public class OtpTextNowTask : IOtpTextNowTask
{
    private readonly IOrderHistoryRepository _orderHistoryRepository;
    private readonly IOtpTextNowClient _otpTextNowClient;

    private bool _isBusy;
    private bool _isBusyWallet;

    public OtpTextNowTask(IOrderHistoryRepository orderHistoryRepository, IOtpTextNowClient otpTextNowClient,
        ITransactionRepository transactionRepository, UserManager<AppUser> userManager)
    {
        _orderHistoryRepository = orderHistoryRepository;
        _otpTextNowClient = otpTextNowClient;
    }

    public async Task ExecuteAsync()
    {
        try
        {
            if (!_isBusy)
            {
                var orderHistories =
                    await _orderHistoryRepository.FindAll(x =>
                            x.Status != OrderStatus.Error && x.Status != OrderStatus.Success)
                        .Take(100)
                        .OrderBy(x => x.CreatedDate)
                        .ToListAsync();
                var orderRequestIds = orderHistories.Select(x => x.Id).ToList();
                if (orderRequestIds.Any())
                {
                    foreach (var item in orderRequestIds)
                    {
                        await _otpTextNowClient.CheckOtpRequest(item.ToString());
                        _isBusy = true;
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            _isBusy = false;
        }
    }
}