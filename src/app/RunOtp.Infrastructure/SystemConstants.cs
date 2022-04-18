namespace RunOtp.Infrastructure;

public class SystemConstants
{
    public const string ConnectionString = "DefaultConnection";
    public const string Admin = "Admin";


    public class TokenProvider
    {
        public const string EmailConfirm = "EmailConfirmationTokenProvider";
        public const string Passwordless = "passwordless-auth";
    }

    public class AuthenticationScheme
    {
        public const string AdminSide = "AdminSide";
        public const string ClientSide = "ClientSide";
    }

    public class UserClaim
    {
        public const string Id = "id";
        public const string FullName = "fullName";
        public const string Avatar = "avatar";
        public const string UserName = "username";
        public const string Role = "role";
    }
}