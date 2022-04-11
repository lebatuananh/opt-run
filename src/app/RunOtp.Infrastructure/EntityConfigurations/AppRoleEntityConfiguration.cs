using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RunOtp.Domain.RoleAggregate;

namespace RunOtp.Infrastructure.EntityConfigurations;

public class AppRoleEntityConfiguration: IEntityTypeConfiguration<AppRole>
{
    public void Configure(EntityTypeBuilder<AppRole> builder)
    {
        builder.ToTable("app_role", MainDbContext.SchemaName);
        builder.HasKey(x => x.Id);
    }
}