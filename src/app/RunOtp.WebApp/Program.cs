using AuditLogging.EntityFramework.Entities;
using RunOtp.Infrastructure;
using RunOtp.WebApp;

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
        .AddPersistence(builder.Configuration)
        .AddIdentityFramework()
        .AddCustomCors()
        .AddAuthenticationCustom()
        .AddHttpContextAccessor()
        .AddHttpClient(builder.Configuration)
        .AddCustomMediatR(new[] { typeof(Anchor) })
        .AddCustomValidators(new[] { typeof(Anchor) })
        .AddRepository()
        .AddAuditEventLogging<MainDbContext, AuditLog>(builder.Configuration)
        .AddInitializationStages()
        .AddControllersWithViews()
        .AddRazorRuntimeCompilation();
    ;
    var app = builder.Build();

    // https://github.com/npgsql/efcore.pg/issues/2158
    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment()) app.UseExceptionHandler("/Error");
    app.MapGet("/error", () => Results.Problem("An error occurred.", statusCode: 500))
        .ExcludeFromDescription();
    app.UseStaticFiles();
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