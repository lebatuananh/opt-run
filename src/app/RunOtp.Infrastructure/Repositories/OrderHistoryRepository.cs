using RunOtp.Domain.OrderHistory;

namespace RunOtp.Infrastructure.Repositories;

public class OrderHistoryRepository : RepositoryIdentity<OrderHistory, MainDbContext>, IOrderHistoryRepository
{
    public OrderHistoryRepository(MainDbContext dbContext) : base(dbContext)
    {
    }
}