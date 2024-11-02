namespace RentalCheckIn.Dtos;

public class HostLoginDto
{
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
}
