namespace RentalCheckIn.Entities;
public class ReservationTranslation
{
    public uint ReservationTranslationId { get; set; }
    public string? CheckInTime { get; set; }
    public string? CheckOutTime { get; set; }
    public uint ReservationId { get; set; }
    public uint LanguageId { get; set; }
    public virtual Reservation Reservation { get; set; } = null!;
    public virtual Language Language { get; set; } = null!;
}
