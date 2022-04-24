using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using RunOtp.Domain.RoleAggregate;
using RunOtp.Domain.UserAggregate;
using RunOtp.Infrastructure;
using RunOtp.Infrastructure.Configurations;

namespace RunOtp.WebApi.UseCase.Users;

public struct Token
{
    public record CreateTokenCommand() : ICreateCommand
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        internal class Validator : AbstractValidator<CreateTokenCommand>
        {
            public Validator()
            {
                RuleFor(x => x.UserName)
                    .NotNull()
                    .NotEmpty()
                    .WithMessage("UserName is not empty");
                RuleFor(x => x.Password)
                    .NotEmpty()
                    .NotNull()
                    .WithMessage("Password is not empty");
            }
        }
    }

    internal class Handler : IRequestHandler<CreateTokenCommand, IResult>
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly TokenConfiguration _tokenConfiguration;

        public Handler(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager,
            TokenConfiguration tokenConfiguration, RoleManager<AppRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _tokenConfiguration = tokenConfiguration;
            _roleManager = roleManager;
        }

        public async Task<IResult> Handle(CreateTokenCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            var role = await _userManager.GetRolesAsync(user);
            if (user == null) return Results.NotFound($"Không tìm thấy tài khoản {request.UserName}");
            if (user.Status == UserStatus.InActive)
            {
                throw new Exception("Account has not been activated or locked, please contact admin for support");
            }
            var result = await _signInManager.PasswordSignInAsync(request.UserName, request.Password, false, true);
            if (!result.Succeeded)
                return Results.BadRequest("Mật khẩu không đúng");
            var claims = new[]
            {
                new Claim("Email", user.Email),
                new Claim(SystemConstants.UserClaim.Id, user.Id.ToString()),
                new Claim(SystemConstants.UserClaim.Avatar, user.Avatar ?? string.Empty),
                new Claim(SystemConstants.UserClaim.UserName, user.UserName),
                new Claim(SystemConstants.UserClaim.FullName, user.FullName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(SystemConstants.UserClaim.Role, string.Join(',', role))
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenConfiguration.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_tokenConfiguration.Issuer,
                _tokenConfiguration.Issuer,
                claims,
                expires: DateTime.Now.AddDays(2),
                signingCredentials: creds);

            return Results.Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }
    }
}