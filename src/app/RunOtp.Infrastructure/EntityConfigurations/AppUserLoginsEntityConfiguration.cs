using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RunOtp.Infrastructure.EntityConfigurations;

public class AppUserLoginsEntityConfiguration : IEntityTypeConfiguration<IdentityUserLogin<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityUserLogin<Guid>> builder)
    {
        builder.ToTable("app_user_logins", MainDbContext.SchemaName);
        builder.HasKey(x => x.UserId);
    }
}