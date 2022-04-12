using Microsoft.AspNetCore.Identity;
using RunOtp.Domain.RoleAggregate;
using RunOtp.Domain.UserAggregate;
using RunOtp.Infrastructure;

namespace RunOtp.WebApp.Initialization;

public class SeedDb : IStage
{
    private readonly MainDbContext _dbContext;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;

    public SeedDb(
        MainDbContext dbContext,
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public int Order => 1;

    public async Task ExecuteAsync()
    {
        await _dbContext.Database.MigrateAsync();
        if (!_roleManager.Roles.Any())
        {
            await _roleManager.CreateAsync(new AppRole()
            {
                Name = "Admin",
                NormalizedName = "Admin",
                Description = "Top manager"
            });
            await _roleManager.CreateAsync(new AppRole()
            {
                Name = "Staff",
                NormalizedName = "Staff",
                Description = "Staff"
            });
            await _roleManager.CreateAsync(new AppRole()
            {
                Name = "Customer",
                NormalizedName = "Customer",
                Description = "Customer"
            });
        }

        if (!_userManager.Users.Any())
        {
            await _userManager.CreateAsync(new AppUser()
            {
                UserName = "admin",
                FullName = "Administrator",
                Email = "admin@gmail.com",
                Balance = 0,
                Status = UserStatus.Active
            }, "123654$");
            var user = await _userManager.FindByNameAsync("admin");
            await _userManager.AddToRoleAsync(user, "Admin");
        }
    }
}