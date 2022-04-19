using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RunOtp.Domain.OrderHistory;
using RunOtp.Domain.UserAggregate;

namespace RunOtp.Infrastructure.EntityConfigurations;

public class OrderHistoryEntityConfiguration : IEntityTypeConfiguration<OrderHistory>
{
    public void Configure(EntityTypeBuilder<OrderHistory> builder)
    {
        builder.ToTable("order_history", MainDbContext.SchemaName);
        builder.HasOne(t => t.AppUser)
            .WithMany(x => x.OrderHistories)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}