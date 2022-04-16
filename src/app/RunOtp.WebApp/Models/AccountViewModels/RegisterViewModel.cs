using System.ComponentModel.DataAnnotations;

namespace RunOtp.WebApp.Models.AccountViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Mời bạn nhập tên", AllowEmptyStrings = false)]
    [Display(Name = "FullName")]
    public string? FullName { set; get; }

    [Display(Name = "DOB")] public DateTime? BirthDay { set; get; }

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
        MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string? Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "ConfirmPassword")]
    [Compare("Password", ErrorMessage = "Mật khẩu không khớp")]
    public string? ConfirmPassword { get; set; }

    [Display(Name = "Address")] public string? Address { get; set; }

    [Display(Name = "Phone number")] public string? PhoneNumber { set; get; }

    [Display(Name = "Avatar")] public string? Avatar { get; set; }
}