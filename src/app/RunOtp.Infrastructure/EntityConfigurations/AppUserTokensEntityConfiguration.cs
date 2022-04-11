using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RunOtp.Infrastructure.EntityConfigurations;

public class AppUserTokensEntityConfiguration : IEntityTypeConfiguration<IdentityUserToken<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityUserToken<Guid>> builder)
    {
        builder.ToTable("app_user_tokens", MainDbContext.SchemaName);
        builder.HasKey(x => x.UserId);
    }
}