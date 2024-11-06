namespace RentalCheckIn.DTOs;

public class PasswordResetDto
{
    [Required(ErrorMessage = "New Password is required")]
    [MinLength(8, ErrorMessage = "Password must be at least 6 characters long")]
    public string NewPassword { get; set; }

    [Required(ErrorMessage = "Confirm Password is required")]
    [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; }
}
