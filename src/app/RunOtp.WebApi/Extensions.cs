using System.Reflection;
using System.Text;
using Hangfire;
using Hangfire.Common;
using Hangfire.PostgreSql;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using RunOtp.Domain.ErrorLog;
using RunOtp.Domain.OrderHistory;
using RunOtp.Domain.RoleAggregate;
using RunOtp.Domain.TransactionAggregate;
using RunOtp.Domain.UserAggregate;
using RunOtp.Domain.WebConfigurationAggregate;
using RunOtp.Infrastructure;
using RunOtp.Infrastructure.Configurations;
using RunOtp.Infrastructure.Repositories;
using RunOtp.WebApi.Services;
using RunOtp.WebApi.Tasks;

namespace RunOtp.WebApi;

public static class Extensions
{
    public static readonly string
        AssemblyName = typeof(Extensions).GetTypeInfo().Assembly.GetName().Name ?? string.Empty;

    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        // TODO: MainDbContext
        services.AddDbContextFactory<MainDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString(ConfigurationKeys.DefaultConnectionString), b =>
            {
                b.MigrationsAssembly(AssemblyName);
                b.MigrationsHistoryTable("__EFMigrationsHistory", MainDbContext.SchemaName);
                b.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
            }).UseSnakeCaseNamingConvention();
            options.UseModel(MainDbContextModel.Instance);
        });

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<MainDbContext>());
        services.AddTransient<IScopeContext, ScopeContext>();
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

    public static IServiceCollection AddAuthenticationCustom(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAuthentication(o =>
        {
            o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(cfg =>
        {
            cfg.RequireHttpsMetadata = false;
            cfg.SaveToken = true;

            cfg.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = configuration["Tokens:Issuer"],
                ValidAudience = configuration["Tokens:Issuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Tokens:Key"]))
            };
        });
        return services;
    }


    public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(AuthorizationConsts.AdministrationPolicy,
                policy =>
                    policy.RequireAssertion(context => context.User.HasClaim(c =>
                            c.Type == $"http://schemas.microsoft.com/ws/2008/06/identity/claims/{JwtClaimTypes.Role}" &&
                            c.Value == AuthorizationConsts.AdminRole ||
                            c.Type == JwtClaimTypes.Role && c.Value == AuthorizationConsts.AdminRole ||
                            c.Type == $"client_{JwtClaimTypes.Role}" &&
                            c.Value == AuthorizationConsts.AdminRole
                        )
                    ));
        });
        return services;
    }

    public static IServiceCollection AddRepository(this IServiceCollection services)
    {
        services.AddScoped<UserManager<AppUser>, UserManager<AppUser>>();
        services.AddScoped<RoleManager<AppRole>, RoleManager<AppRole>>();
        services.AddTransient<IWebConfigurationRepository, WebConfigurationRepository>();
        services.AddTransient<IOrderHistoryRepository, OrderHistoryRepository>();
        services.AddTransient<ITransactionRepository, TransactionRepository>();
        services.AddTransient<IErrorLogRepository, ErrorLogRepository>();
        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddTransient<IOtpExternalService, OtpExternalService>();
        return services;
    }

    public static IServiceCollection AddHangFireCustom(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfire(config =>
        {
            var jobStorageConnectionString =
                configuration.GetConnectionString(ConfigurationKeys.ScheduledTasksDbConnectionString);
            config.UsePostgreSqlStorage(jobStorageConnectionString, new PostgreSqlStorageOptions()
            {
                SchemaName = "scheduled_tasks",
                PrepareSchemaIfNecessary = true
            });
        });

        services.AddSingleton<IBackgroundJobClient>((x =>
            new BackgroundJobClient(x.GetRequiredService<JobStorage>(),
                x.GetRequiredService<IJobFilterProvider>())));
        // Add the processing server as IHostedService
        services.AddHangfireServer();
        return services;
    }

    public static IServiceCollection AddConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddSingleton(sp => configuration.GetSection("Tokens")
                .Get<TokenConfiguration>());

        return services;
    }

    public static IServiceCollection AddSwaggerConfig(this IServiceCollection services,
        IConfiguration configuration)
    {
        var settings = configuration.GetSection("Authentication").Get<AuthenticationSettings>();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Description = "RunOtp web api in Asp.Net Core",
                Title = "RunOtp Api",
                Version = "v1",
                Contact = new OpenApiContact
                {
                    Name = "MRA",
                    Url = new Uri("http://netcoreexamples.com")
                }
            });
            // To Enable authorization using Swagger (JWT)  
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description =
                    "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    new List<string> { "runOtp" }
                }
            });
        });
        return services;
    }
}