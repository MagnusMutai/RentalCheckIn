namespace RentalCheckIn.DTOs;
public class CheckInReservationUpdateDTO
{
    public uint Id { get; set; }
    public string? PassportNr { get; set; }
    public string? MailAddress { get; set; }
    public string? Mobile { get; set; }
    public decimal ApartmentFee { get; set; } // Editable
    public decimal SecurityDeposit { get; set; } // Editable
    public decimal? TotalPrice { get; set; } // Calculated
    public decimal? KwhAtCheckIn { get; set; } // Editable
    public string? SignatureDataUrl { get; set; } // Editable (guest's signature)
}
