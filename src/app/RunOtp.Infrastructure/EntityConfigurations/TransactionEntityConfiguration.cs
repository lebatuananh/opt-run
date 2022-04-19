using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RunOtp.Domain.TransactionAggregate;

namespace RunOtp.Infrastructure.EntityConfigurations;

public class TransactionEntityConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("transaction", MainDbContext.SchemaName);
        builder.HasOne(t => t.AppUser)
            .WithMany(x => x.Transactions)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}