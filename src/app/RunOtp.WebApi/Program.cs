using AuditLogging.EntityFramework.Entities;
using RunOtp.Infrastructure;
using RunOtp.WebApi;

await WithSeriLog(async () =>
{
    var builder = WebApplication.CreateBuilder(args);
    var configuration = builder.Configuration;
    configuration.AddJsonFile("serilog.json", optional: true, reloadOnChange: true);
    configuration.AddEnvironmentVariables();
    configuration.AddCommandLine(args);
    builder.Host.AddSerilog("RunOtp");
    // Add services to the container.
    builder.Services
        .AddCustomCors()
        .AddHttpContextAccessor()
        .AddHttpClient(builder.Configuration)
        .AddIdentityFramework()
        .AddCustomMediatR(new[] { typeof(Anchor) })
        .AddCustomValidators(new[] { typeof(Anchor) })
        .AddAuthenticationCustom(builder.Configuration)
        .AddSwaggerConfig(builder.Configuration)
        .AddPersistence(builder.Configuration)
        .AddConfig(builder.Configuration)
        .AddRepository()
        .AddConfig(builder.Configuration)
        .AddAuditEventLogging<MainDbContext, AuditLog>(builder.Configuration)
        .AddEndpointsApiExplorer()
        .AddInitializationStages()
        .AddControllers();
    var app = builder.Build();

    // https://github.com/npgsql/efcore.pg/issues/2158
    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment()) app.UseExceptionHandler("/Error");
    app.MapGet("/error", () => Results.Problem("An error occurred.", statusCode: 500))
        .ExcludeFromDescription();

    app.UseMiddleware<ExceptionMiddleware>();
    app.UseCustomCors();
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapFallback(() => Results.Redirect("/swagger"));
    app.MapControllers();
    await app.AutoInit(app.Logger);
    app.Run();
});