using RentalCheckIn.Locales;
namespace RentalCheckIn.DTOs;

// There are properties like Email which are repeated everywhere. Consolidate duplicate code.
public class ResetRequestDTO
{
    [Required(ErrorMessageResourceName = "EmailRequired", ErrorMessageResourceType = typeof(Resource))]
    [EmailAddress]
    public string Email { get; set; }
}
