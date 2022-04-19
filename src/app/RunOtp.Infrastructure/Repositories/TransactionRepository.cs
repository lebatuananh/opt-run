using RunOtp.Domain.TransactionAggregate;

namespace RunOtp.Infrastructure.Repositories;

public class TransactionRepository : RepositoryIdentity<Transaction, MainDbContext>, ITransactionRepository
{
    public TransactionRepository(MainDbContext dbContext) : base(dbContext)
    {
    }
}