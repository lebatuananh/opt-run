using Microsoft.AspNetCore.Identity;
using RunOtp.Domain.TransactionAggregate;
using RunOtp.Domain.UserAggregate;
using RunOtp.Infrastructure;

namespace RunOtp.WebApi.Tasks;

public class TransactionHostedServices : IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public TransactionHostedServices(IServiceScopeFactory scopeFactory)
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
                    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                    var transactionRepository = scope.ServiceProvider.GetRequiredService<ITransactionRepository>();
                    var transactions = await transactionRepository.FindAll(
                            x => !string.IsNullOrEmpty(x.Ref) && x.Status != TransactionStatus.Completed)
                        .Take(100)
                        .OrderBy(x => x.CreatedDate).ToListAsync(cancellationToken: cancellationToken);
                    if (transactions.Count > 0)
                    {
                        foreach (var item in transactions)
                        {
                            var user = await userManager.FindByIdAsync(item.UserId.ToString());
                            if (user is null)
                            {
                                throw new Exception("User not found");
                            }

                            user.SubtractMoneyOtp();
                            var result = await userManager.UpdateAsync(user);
                            if (result.Succeeded)
                            {
                                item.MarkCompleted();
                            }
                            else
                            {
                                item.MarkError();
                            }

                            transactionRepository.Update(item);
                        }

                        await transactionRepository.CommitAsync();
                    }
                }

                await Task.Delay(new TimeSpan(0, 0, 5), cancellationToken); // 5 second delay
            }
        }, cancellationToken);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}