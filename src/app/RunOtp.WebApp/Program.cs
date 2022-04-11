using AuditLogging.EntityFramework.Entities;
using RunOtp.WebApp;

await WithSeriLog(async () =>
{
    var builder = WebApplication.CreateBuilder(args);
    var configuration = builder.Configuration;
    configuration.AddJsonFile("serilog.json", optional: true, reloadOnChange: true);
    configuration.AddEnvironmentVariables();
    configuration.AddCommandLine(args);
    builder.Host.AddSerilog("GithubTrending");
    // Add services to the container.
    builder.Services
        .AddCustomCors()
        .AddHttpContextAccessor()
        .AddHttpClient(builder.Configuration)
        .AddCustomMediatR(new[] { typeof(Anchor) })
        .AddCustomValidators(new[] { typeof(Anchor) })
        .AddPersistence(builder.Configuration)
        .AddRepository()
        .AddAuditEventLogging<AuditLogDbContext, AuditLog>(builder.Configuration)
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
    app.MapControllers();
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    await app.AutoInit(app.Logger);
    app.Run();
});