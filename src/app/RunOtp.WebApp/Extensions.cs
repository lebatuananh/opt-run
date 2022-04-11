using System.Reflection;
using Microsoft.AspNetCore.Identity;
using RunOtp.Domain.RoleAggregate;
using RunOtp.Domain.UserAggregate;
using RunOtp.Infrastructure;

namespace RunOtp.WebApp;

public static class Extensions
{
    public static readonly string
        AssemblyName = typeof(Extensions).GetTypeInfo().Assembly.GetName().Name ?? string.Empty;

    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        // TODO: MainDbContext
        services.AddDbContext<MainDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString(ConfigurationKeys.DefaultConnectionString), b =>
            {
                b.MigrationsAssembly(AssemblyName);
                b.MigrationsHistoryTable("__EFMigrationsHistory", MainDbContext.SchemaName);
                b.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
            }).UseSnakeCaseNamingConvention();
            // options.UseModel(MainDbContextModel.Instance);
        });

        // TODO: AuditLogDbContext
        services.AddDbContext<AuditLogDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString(ConfigurationKeys.DefaultConnectionString), b =>
            {
                b.MigrationsAssembly(AssemblyName);
                b.MigrationsHistoryTable("__EFMigrationsHistory", AuditLogDbContext.SchemaName);
                b.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
            }).UseSnakeCaseNamingConvention();
            // options.UseModel(AuditLogDbContextModel.Instance);
        });

        // TODO: LogDbContext
        services.AddDbContext<LogDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString(ConfigurationKeys.DefaultConnectionString), b =>
            {
                b.MigrationsAssembly(AssemblyName);
                b.MigrationsHistoryTable("__EFMigrationsHistory", LogDbContext.SchemaName);
                b.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
            }).UseSnakeCaseNamingConvention();
            // options.UseModel(LogDbContextModel.Instance);
        });

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<MainDbContext>());
        services.AddScoped<IScopeContext, ScopeContext>();
        return services;
    }

    public static IServiceCollection AddIdentityFramework(this IServiceCollection services)
    {
        services.AddIdentity<AppUser, AppRole>()
            .AddEntityFrameworkStores<MainDbContext>()
            .AddDefaultTokenProviders();

        // Configure Identity
        services.Configure<IdentityOptions>(options =>
        {
            // Password settings
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;

            // Lockout settings
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
            options.Lockout.MaxFailedAccessAttempts = 10;

            // User settings
            options.User.RequireUniqueEmail = true;
        });
        return services;
    }

    public static IServiceCollection AddRepository(this IServiceCollection services)
    {
        services.AddScoped<UserManager<AppUser>, UserManager<AppUser>>();
        services.AddScoped<RoleManager<AppRole>, RoleManager<AppRole>>();
        services.AddScoped<ILogRepository, LogRepository>();
        return services;
    }
}