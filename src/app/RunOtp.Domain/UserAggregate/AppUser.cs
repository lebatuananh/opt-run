using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.SeedWork;

namespace RunOtp.Domain.UserAggregate;

public class AppUser : IdentityUser<Guid>, IAggregateRoot
{
    public AppUser()
    {
    }

    public AppUser(string? fullName, DateTime? birthDay, decimal balance, string? avatar, UserStatus status)
    {
        FullName = fullName;
        BirthDay = birthDay;
        Balance = balance;
        Avatar = avatar;
        Status = status;
    }

    public AppUser(string userName, string? fullName, DateTime? birthDay, decimal balance, string? avatar,
        UserStatus status) : base(userName)
    {
        FullName = fullName;
        BirthDay = birthDay;
        Balance = balance;
        Avatar = avatar;
        Status = status;
    }

    public string? FullName { get; set; }

    public DateTime? BirthDay { set; get; }

    public decimal Balance { get; set; }

    public string? Avatar { get; set; }

    public UserStatus Status { get; set; }
}