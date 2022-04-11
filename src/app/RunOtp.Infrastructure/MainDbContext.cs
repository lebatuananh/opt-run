using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RunOtp.Domain.RoleAggregate;
using RunOtp.Domain.UserAggregate;
using Shared.Constants;
using Shared.Extensions;
using Shared.SeedWork;

namespace RunOtp.Infrastructure;

public class MainDbContext : IdentityDbContext<AppUser, AppRole, Guid>, IUnitOfWork
{
    public static string SchemaName => "data";

    protected readonly IMediator Mediator;
    protected readonly IScopeContext ScopeContext;

    public MainDbContext()
    {
    }

    protected MainDbContext(DbContextOptions options) : base(options)
    {
    }

    protected MainDbContext(
        DbContextOptions options,
        IMediator mediator,
        IScopeContext scopeContext) : base(options)
    {
        Mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        ScopeContext = scopeContext ?? throw new ArgumentNullException(nameof(scopeContext));
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema(SchemaName);
        builder.HasPostgresExtension(PostgresDefaultAlgorithm.UuidGenerator);
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AddTrack();
        var result = await base.SaveChangesAsync(cancellationToken);
        await Mediator.DispatchDomainEventsAsync(this);
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
                        modifier.MarkCreated(ScopeContext.CurrentAccountId, ScopeContext.CurrentAccountName);
                    if (entry.Entity is IDateTracking ent) ent.MarkCreated();

                    if (entry.Entity.GetType().GetCustomAttribute<PredefinedObjectAttribute>() != null)
                        entry.State = EntityState.Unchanged;
                    break;
                case EntityState.Modified:
                    if (entry.Entity is ModifierTrackingEntity modifier2)
                        modifier2.MarkModified(ScopeContext.CurrentAccountId, ScopeContext.CurrentAccountName);
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
}