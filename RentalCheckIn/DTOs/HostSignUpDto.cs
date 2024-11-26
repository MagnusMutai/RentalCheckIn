﻿namespace RentalCheckIn.DTOs;
using RentalCheckIn.Locales;

[MetadataType(typeof(Resource))]
public class HostSignUpDTO
{
    [Required(ErrorMessageResourceName = "FirstNameRequired", ErrorMessageResourceType = typeof(Resource))]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessageResourceName = "LastNameRequired", ErrorMessageResourceType = typeof(Resource))]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessageResourceName = "EmailRequired", ErrorMessageResourceType = typeof(Resource))]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessageResourceName = "PasswordRequired", ErrorMessageResourceType = typeof(Resource))]
    [DataType(DataType.Password)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        ErrorMessageResourceName = "PasswordRegexError", ErrorMessageResourceType = typeof(Resource))]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessageResourceName = "ConfirmPasswordCompareError", ErrorMessageResourceType = typeof(Resource))]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessageResourceName = "Selected2FARequired", ErrorMessageResourceType = typeof(Resource))]
    [RegularExpression("^(TOTP|FaceID)$", ErrorMessageResourceName = "Selected2FARegexError", ErrorMessageResourceType = typeof(Resource))]
    public string Selected2FA { get; set; } = string.Empty;

}
