using RunOtp.Domain.OrderHistory;
using RunOtp.Domain.WebConfigurationAggregate;
using RunOtp.Driver.OtpTextNow;
using RunOtp.Driver.RentOtp;

namespace RunOtp.WebApi.Tasks;

public class OtpTextNowHostedServices : IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public OtpTextNowHostedServices(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(async () =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var orderHistoryRepository = scope.ServiceProvider.GetRequiredService<IOrderHistoryRepository>();
                    var otpTextNowClient = scope.ServiceProvider.GetRequiredService<IOtpTextNowClient>();
                    var rentTextNowClient = scope.ServiceProvider.GetRequiredService<IRentCodeTextNowClient>();

                    var orderHistories =
                        await orderHistoryRepository.FindAll(x =>
                                x.Status != OrderStatus.Error && x.Status != OrderStatus.Success)
                            .Take(300)
                            .OrderBy(x => x.CreatedDate)
                            .ToListAsync(cancellationToken: cancellationToken);
                    if (orderHistories.Any())
                    {
                        foreach (var item in orderHistories)
                        {
                            switch (item.WebType)
                            {
                                case WebType.RentOtp:
                                    await rentTextNowClient.CheckOtpRequest(item.ToString());
                                    break;
                                case WebType.OtpTextNow:
                                    await otpTextNowClient.CheckOtpRequest(item.ToString());
                                    break;
                                case WebType.RunOtp:
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }
                    }
                }

                await Task.Delay(new TimeSpan(0, 0, 10), cancellationToken); // 5 second delay
            }
        }, cancellationToken);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}