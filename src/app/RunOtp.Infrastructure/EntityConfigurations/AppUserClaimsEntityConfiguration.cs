using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RunOtp.Infrastructure.EntityConfigurations;

public class AppUserClaimsEntityConfiguration: IEntityTypeConfiguration<IdentityUserClaim<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityUserClaim<Guid>> builder)
    {
        builder.ToTable("app_user_claims", MainDbContext.SchemaName);
        builder.HasKey(x => x.Id);
    }
}