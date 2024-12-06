namespace RentalCheckIn.DTOs;
public class CheckInReservationUpdateDTO
{
    public uint Id { get; set; }
    public uint LHostId { get; set; }
    public string? PassportNr { get; set; }
    public string? MailAddress { get; set; }
    public string? Mobile { get; set; }
    public decimal ApartmentFee { get; set; } 
    public decimal SecurityDeposit { get; set; }
    // Calculated
    public decimal? TotalPrice { get; set; }
    public int KwhAtCheckIn { get; set; }
    // Checkboxes
    public bool AgreeEnergyConsumption { get; set; } = true;
    public bool ReceivedKeys { get; set; } = true;
    public bool AgreeTerms { get; set; } = true;
    // Editable (guest's signature)
    public string? SignatureDataUrl { get; set; }
}
