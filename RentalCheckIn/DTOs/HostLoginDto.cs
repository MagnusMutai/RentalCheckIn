namespace RentalCheckIn.DTOs;

public class HostLoginDto
{
    [Required(ErrorMessage ="Email is required"), EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required(ErrorMessage = "Password is required"), DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}
