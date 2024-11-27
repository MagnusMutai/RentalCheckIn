using RentalCheckIn.Locales;

namespace RentalCheckIn.DTOs;

// Finally there are properties like emails which are repeated everywhere, chop unnecessary code
public class ResetRequestDTO
{
    [Required(ErrorMessageResourceName = "EmailRequired", ErrorMessageResourceType = typeof(Resource))]
    [EmailAddress]
    public string Email { get; set; }
}
