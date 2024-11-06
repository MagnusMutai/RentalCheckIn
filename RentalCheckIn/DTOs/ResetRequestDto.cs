namespace RentalCheckIn.DTOs;

// Finally there are properties like emails which are repeated everywhere, chop unnecessary code
public class ResetRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}
