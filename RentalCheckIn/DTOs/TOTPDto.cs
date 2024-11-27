using RentalCheckIn.Locales;

namespace RentalCheckIn.DTOs;
public class TOTPDTO
{
    public string Email { get; set; }
    [Required(ErrorMessageResourceName = "OTPCodeRequired", ErrorMessageResourceType = typeof(Resource))]
    public string Code { get; set; }
}
