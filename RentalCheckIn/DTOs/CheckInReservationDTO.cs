namespace RentalCheckIn.DTOs;

public class CheckInReservationDTO
{

    // Guest Information
    public uint Id { get; set; }
    public string GuestFullName { get; set; }
    public string GuestFirstName { get; set; }

    [Required]
    [MaxLength(50)]
    public string PassportNr { get; set; }

    [EmailAddress]
    public string MailAddress { get; set; }

    [Required]
    public string Mobile { get; set; }

    // Reservation Information
    [Required]
    public DateOnly CheckInDate { get; set; }

    [Required]
    public DateOnly CheckOutDate { get; set; }
    // Reservation Information
    [Required]
    public string CheckInTime { get; set; }

    [Required]
    public string CheckOutTime { get; set; }

    public int NumberOfNights { get; set; }
    public int NumberOfGuests { get; set; }

    [Required]
    public string ApartmentName { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Apartment fee must be greater than 0")]
    public decimal ApartmentFee { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Security deposit must be greater than 0")]
    public decimal SecurityDeposit { get; set; }

    public decimal? TotalPrice { get; set; }

    // Meter kWh
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Meter reading must be greater than 0")]
    public int KwhAtCheckIn { get; set; }

    // Checkboxes
    public bool? AgreeEnergyConsumption { get; set; }
    public bool? ReceivedKeys { get; set; }
    public bool? AgreeTerms { get; set; }

    // Signature
    [Required(ErrorMessage = "Signature is required")]
    public string SignatureDataUrl { get; set; }

    // Additional Information

    public string CurrencySymbol { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (CheckOutDate <= CheckInDate)
        {
            yield return new ValidationResult("Check-Out Date must be after Check-In Date", new[] { nameof(CheckOutDate) });
        }

        if (NumberOfNights <= 0)
        {
            yield return new ValidationResult("Number of nights must be greater than 0", new[] { nameof(NumberOfNights) });
        }
    }

}
