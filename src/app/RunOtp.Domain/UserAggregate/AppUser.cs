using Microsoft.AspNetCore.Identity;
using RunOtp.Domain.TransactionAggregate;
using Shared.SeedWork;

namespace RunOtp.Domain.UserAggregate;

public class AppUser : IdentityUser<Guid>, IAggregateRoot
{
    public AppUser()
    {
    }

    public AppUser(string fullName, DateTime? birthDay, string avatar, UserStatus status)
    {
        FullName = fullName;
        BirthDay = birthDay;
        Balance = 0;
        Avatar = avatar;
        Status = status;
        Discount = 0;
    }

    public AppUser(string userName, string email, string fullName, DateTime? birthDay, string avatar,
        UserStatus status) : base(userName)
    {
        FullName = fullName;
        BirthDay = birthDay;
        Avatar = avatar;
        Status = status;
        Email = email;
        Balance = 0;
        Discount = 0;
    }

    public void Recharge(decimal amount)
    {
        Balance += amount;
        Deposit += amount;
    }

    public void SubtractMoney(decimal amount)
    {
        Balance -= amount;
        TotalAmountUsed += amount;
    }

    public void Enable()
    {
        Status = UserStatus.Active;
    }

    public void Disable()
    {
        Status = UserStatus.InActive;
    }

    public string FullName { get; set; }

    public DateTime? BirthDay { set; get; }

    public decimal Balance { get; set; }
    public decimal TotalAmountUsed { get; set; }
    public decimal Deposit { get; set; }
    public string Avatar { get; set; }
    public int Discount { get; set; }
    public string ClientSecret { get; set; }
    public virtual IList<Transaction> Transactions { get; private set; }


    public UserStatus Status { get; set; }
}