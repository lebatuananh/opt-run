using RunOtp.Domain.OrderHistory;
using RunOtp.Driver.OtpTextNow;

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

                    var orderHistories =
                        await orderHistoryRepository.FindAll(x =>
                                x.Status != OrderStatus.Error && x.Status != OrderStatus.Success)
                            .Take(100)
                            .OrderBy(x => x.CreatedDate)
                            .ToListAsync(cancellationToken: cancellationToken);
                    var orderRequestIds = orderHistories.Select(x => x.Id).ToList();
                    if (orderRequestIds.Any())
                    {
                        foreach (var item in orderRequestIds)
                        {
                            await otpTextNowClient.CheckOtpRequest(item.ToString());
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