using System.Reflection;
using AuditLogging.EntityFramework.DbContexts;
using AuditLogging.EntityFramework.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RunOtp.Domain.OrderHistory;
using RunOtp.Domain.RoleAggregate;
using RunOtp.Domain.TransactionAggregate;
using RunOtp.Domain.UserAggregate;
using RunOtp.Domain.WebConfigurationAggregate;
using Shared.Constants;
using Shared.Extensions;
using Shared.Logging.LogError;
using Shared.SeedWork;
using StackExchange.Redis;

namespace RunOtp.Infrastructure;

public class MainDbContext : IdentityDbContext<AppUser, AppRole, Guid>, IUnitOfWork, IAuditLoggingDbContext<AuditLog>
{
    public static string SchemaName => "data";

    private readonly IMediator _mediator;
    private readonly IScopeContext _scopeContext;

    public MainDbContext()
    {
    }
    
    public MainDbContext(DbContextOptions options) : base(options)
    {
    }

    public MainDbContext(
        DbContextOptions options,
        IMediator mediator,
        IScopeContext scopeContext) : base(options)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _scopeContext = scopeContext ?? throw new ArgumentNullException(nameof(scopeContext));
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema(SchemaName);
        builder.HasPostgresExtension(PostgresDefaultAlgorithm.UuidGenerator);
        builder.Entity<Log>(log =>
        {
            log.ToTable("log", SchemaName);
            log.HasKey(x => x.Id);
            log.Property(x => x.LogEvent).HasColumnType("jsonb");
            log.Property(x => x.Properties).HasColumnType("jsonb");
        });
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AddTrack();
        var result = await base.SaveChangesAsync(cancellationToken);
        await _mediator.DispatchDomainEventsAsync(this);
        return result;
    }

    private void AddTrack()
    {
        var entities = ChangeTracker.Entries()
            .Where(x => x.State is EntityState.Added or EntityState.Modified);
        foreach (var entry in entities)
            switch (entry.State)
            {
                case EntityState.Added:
                    if (entry.Entity is ModifierTrackingEntity modifier)
                        modifier.MarkCreated(_scopeContext.CurrentAccountId, _scopeContext.CurrentAccountName);
                    if (entry.Entity is IDateTracking ent) ent.MarkCreated();

                    if (entry.Entity.GetType().GetCustomAttribute<PredefinedObjectAttribute>() != null)
                        entry.State = EntityState.Unchanged;
                    break;
                case EntityState.Modified:
                    if (entry.Entity is ModifierTrackingEntity modifier2)
                        modifier2.MarkModified(_scopeContext.CurrentAccountId, _scopeContext.CurrentAccountName);
                    if (entry.Entity is IDateTracking ent2)
                    {
                        ent2.MarkModified();
                        entry.Property("CreatedDate").IsModified = false;
                        entry.Property("LastUpdatedDate").IsModified = true;
                    }

                    if (entry.Entity.GetType().GetCustomAttribute<PredefinedObjectAttribute>() != null)
                        entry.State = EntityState.Unchanged;
                    break;
                case EntityState.Unchanged:
                case EntityState.Detached:
                case EntityState.Deleted:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
    }

    public DbSet<AuditLog> AuditLog { get; set; }
    public DbSet<Log> Logs { get; set; }
    public DbSet<WebConfiguration> WebConfigurations { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<OrderHistory> OrderHistory { get; set; }


    public async Task<int> SaveChangesAsync()
    {
        return await base.SaveChangesAsync();
    }
}