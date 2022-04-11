using Microsoft.AspNetCore.Identity;
using Shared.SeedWork;

namespace RunOtp.Domain.RoleAggregate;

public class AppRole : IdentityRole<Guid>, IAggregateRoot
{
    public AppRole()
    {
    }

    public AppRole(string name, string? description) : base(name)
    {
        Description = description;
    }

    public string? Description { get; set; }
}