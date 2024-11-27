using RentalCheckIn.Locales;

namespace RentalCheckIn.DTOs;

public class PasswordResetDTO
{
    [Required(ErrorMessageResourceName = "PasswordRequired", ErrorMessageResourceType = typeof(Resource))]
    [DataType(DataType.Password)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        ErrorMessageResourceName = "PasswordRegexError", ErrorMessageResourceType = typeof(Resource))]
    public string NewPassword { get; set; }

    [Required(ErrorMessageResourceName = "ConfirmPasswordRequired", ErrorMessageResourceType = typeof(Resource))]
    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessageResourceName = "ConfirmPasswordCompareError", ErrorMessageResourceType = typeof(Resource))]
    public string ConfirmPassword { get; set; }
}
