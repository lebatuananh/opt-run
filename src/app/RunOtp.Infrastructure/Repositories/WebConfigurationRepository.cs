using RunOtp.Domain.WebConfigurationAggregate;

namespace RunOtp.Infrastructure.Repositories;

public class WebConfigurationRepository : RepositoryIdentity<WebConfiguration, MainDbContext>,
    IWebConfigurationRepository
{
    public WebConfigurationRepository(MainDbContext dbContext) : base(dbContext)
    {
    }
}