using RentalCheckIn.Locales;

namespace RentalCheckIn.DTOs;

public class CheckInReservationDTO : IValidatableObject
{
    // Guest Information
    public uint Id { get; set; }
    public uint LHostId { get; set; }
    public string GuestFullName { get; set; }
    public string GuestFirstName { get; set; }

    [Required(ErrorMessageResourceName = "PassportNrRequired", ErrorMessageResourceType = typeof(Resource))]
    [MaxLength(50, ErrorMessageResourceName = "PassportNrMaxLength", ErrorMessageResourceType = typeof(Resource))]
    public string PassportNr { get; set; }

    [EmailAddress(ErrorMessageResourceName = "InvalidEmail", ErrorMessageResourceType = typeof(Resource))]
    public string MailAddress { get; set; }

    [Required(ErrorMessageResourceName = "MobileRequired", ErrorMessageResourceType = typeof(Resource))]
    public string Mobile { get; set; }

    // Reservation Information
    [Required(ErrorMessageResourceName = "CheckInDateRequired", ErrorMessageResourceType = typeof(Resource))]
    public DateOnly CheckInDate { get; set; }

    [Required(ErrorMessageResourceName = "CheckOutDateRequired", ErrorMessageResourceType = typeof(Resource))]
    public DateOnly CheckOutDate { get; set; }

    [Required(ErrorMessageResourceName = "CheckInTimeRequired", ErrorMessageResourceType = typeof(Resource))]
    public string CheckInTime { get; set; }

    [Required(ErrorMessageResourceName = "CheckOutTimeRequired", ErrorMessageResourceType = typeof(Resource))]
    public string CheckOutTime { get; set; }

    public int NumberOfNights { get; set; }
    public int NumberOfGuests { get; set; }

    [Required(ErrorMessageResourceName = "ApartmentNameRequired", ErrorMessageResourceType = typeof(Resource))]
    public string ApartmentName { get; set; }

    [Required(ErrorMessageResourceName = "ApartmentFeeRequired", ErrorMessageResourceType = typeof(Resource))]
    [Range(0.01, double.MaxValue, ErrorMessageResourceName = "ApartmentFeeRange", ErrorMessageResourceType = typeof(Resource))]
    public decimal ApartmentFee { get; set; }

    [Required(ErrorMessageResourceName = "SecurityDepositRequired", ErrorMessageResourceType = typeof(Resource))]
    [Range(0.01, double.MaxValue, ErrorMessageResourceName = "SecurityDepositRange", ErrorMessageResourceType = typeof(Resource))]
    public decimal SecurityDeposit { get; set; }

    public decimal? TotalPrice { get; set; }

    // Meter kWh
    [Required(ErrorMessageResourceName = "KwhAtCheckInRequired", ErrorMessageResourceType = typeof(Resource))]
    [Range(1, int.MaxValue, ErrorMessageResourceName = "KwhAtCheckInRange", ErrorMessageResourceType = typeof(Resource))]
    public int KwhAtCheckIn { get; set; }
    public int KWhPerNightIncluded { get; set; }
    public decimal? CostsPerXtraKWh { get; set; }

    // Checkboxes
    public bool AgreeEnergyConsumption { get; set; }
    public bool ReceivedKeys { get; set; } 
    public bool AgreeTerms { get; set; }

    public string CountryISO2 { get; set; }
    public string LanguageName { get; set; }
    public string SignatureDataUrl { get; set; }
    public string CurrencySymbol { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (CheckOutDate <= CheckInDate)
        {
            yield return new ValidationResult(
                Resource.CheckOutDateAfterCheckIn,
                new[] { nameof(CheckOutDate) }
            );
        }

        if (NumberOfNights <= 0)
        {
            yield return new ValidationResult(
                Resource.NumberOfNightsGreaterThanZero,
                new[] { nameof(NumberOfNights) }
            );
        }
    }
}
