using RunOtp.Domain.OrderHistory;
using RunOtp.Domain.WebConfigurationAggregate;
using RunOtp.Driver.OtpTextNow;
using RunOtp.Driver.RentOtp;
using RunOtp.Driver.RunOtp;
using Log = Serilog.Log;

namespace RunOtp.WebApi.Tasks;

public class CreatedOrderHistoryHostedServices : IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public CreatedOrderHistoryHostedServices(IServiceScopeFactory scopeFactory)
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
                    var runOtpClient = scope.ServiceProvider.GetRequiredService<IRunOtpClient>();

                    var orderHistories =
                        await orderHistoryRepository.FindAll(x => x.Status == OrderStatus.Created)
                            .Take(1000)
                            .OrderBy(x => x.CreatedDate)
                            .ToListAsync(cancellationToken: cancellationToken);
                    if (orderHistories.Any())
                    {
                        foreach (var item in orderHistories)
                        {
                            Log.Error("Request created {Id} - {Phone} - {Web}", item.Id, item.NumberPhone, item.WebType);
                            switch (item.WebType)
                            {
                                case WebType.RentOtp:
                                    await rentTextNowClient.CheckOtpRequest(item);
                                    break;
                                case WebType.OtpTextNow:
                                    await otpTextNowClient.CheckOtpRequest(item);
                                    break;
                                case WebType.RunOtp:
                                    await runOtpClient.CheckOtpRequest(item);
                                    break;
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