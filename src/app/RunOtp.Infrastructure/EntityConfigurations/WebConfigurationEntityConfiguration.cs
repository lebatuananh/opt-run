using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RunOtp.Domain.WebConfigurationAggregate;
using Shared.Constants;

namespace RunOtp.Infrastructure.EntityConfigurations;

public class WebConfigurationEntityConfiguration : IEntityTypeConfiguration<WebConfiguration>
{
    public void Configure(EntityTypeBuilder<WebConfiguration> builder)
    {
        builder.ToTable("web_configuration", MainDbContext.SchemaName);
        builder.Property(x => x.Id).HasColumnType("uuid")
            .HasDefaultValueSql(PostgresDefaultAlgorithm.UuidAlgorithm);
        builder.Property(x => x.ApiSecret).IsRequired().HasMaxLength(128);
        builder.HasKey(x => x.Id);
    }
}