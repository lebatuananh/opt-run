using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RunOtp.Infrastructure.EntityConfigurations;

public class AppRoleClaimsEntityConfiguration : IEntityTypeConfiguration<IdentityRoleClaim<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityRoleClaim<Guid>> builder)
    {
        builder.ToTable("app_role_claims", MainDbContext.SchemaName);
        builder.HasKey(x => x.Id);
    }
}