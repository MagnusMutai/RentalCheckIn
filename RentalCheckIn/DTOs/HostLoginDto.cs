using RentalCheckIn.Locales;
namespace RentalCheckIn.DTOs;

public class HostLoginDTO
{
    [Required(ErrorMessageResourceName = "EmailRequired", ErrorMessageResourceType = typeof(Resource))]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required(ErrorMessageResourceName = "PasswordRequired", ErrorMessageResourceType = typeof(Resource))]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}
