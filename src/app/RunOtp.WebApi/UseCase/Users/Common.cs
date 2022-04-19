namespace RunOtp.WebApi.UseCase.Users;

public record UserDto(Guid Id, string Email, string Username, decimal Balance, decimal TotalAmountUsed,
    decimal Deposit, int Discount, string ClientSecret);