using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RunOtp.Infrastructure.EntityConfigurations;

public class AppUserRolesEntityConfiguration : IEntityTypeConfiguration<IdentityUserRole<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityUserRole<Guid>> builder)
    {
        builder.ToTable("app_user_roles", MainDbContext.SchemaName);
        builder.HasKey(x => new { x.RoleId, x.UserId });
    }
}