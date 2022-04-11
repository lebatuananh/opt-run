using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RunOtp.Domain.UserAggregate;

namespace RunOtp.Infrastructure.EntityConfigurations;

public class AppUserEntityConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.ToTable("app_user", MainDbContext.SchemaName);
        builder.HasKey(x => x.Id);
    }
}