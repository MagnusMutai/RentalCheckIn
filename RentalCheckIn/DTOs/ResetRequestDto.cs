namespace RentalCheckIn.DTOs;

// Finally there are properties like emails which are repeated everywhere, chop unnecessary code
public class ResetRequestDTO
{
    [Required(ErrorMessage = "Error Message is required.")]
    [EmailAddress]
    public string Email { get; set; }
}
