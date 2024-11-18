namespace RentalCheckIn.DTOs;
public class HostSignUpDTO
{
    [Required(ErrorMessage = "First name is required")]
    public string FirstName { get; set; } = string.Empty;
    [Required(ErrorMessage = "Last name is required")]
    public string LastName { get; set; } = string.Empty;
    [Required(ErrorMessage = "Email is required"), EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
    ErrorMessage = "Password must be at least 8 characters long, have an upper letter, a lowercase letter, a number, and a special character.")]
    public string Password { get; set; } = string.Empty;
    [DataType(DataType.Password), Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;
    // Change back to bool
    [Required(ErrorMessage = "Please select a two-factor authentication method.")]
    [RegularExpression("^(TOTP|FaceID)$", ErrorMessage = "Invalid two-factor authentication method selected.")]
    public string Selected2FA { get; set; }

}
