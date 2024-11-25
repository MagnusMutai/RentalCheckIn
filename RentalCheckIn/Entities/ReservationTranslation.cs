using System.ComponentModel.DataAnnotations.Schema;

namespace RentalCheckIn.Entities;
public class ReservationTranslation
{
    public int ReservationTranslationId { get; set; }
    public uint ReservationId { get; set; }
    public uint LanguageId { get; set; }
    public string? CheckInTime { get; set; }
    public string? CheckOutTime { get; set; }

    public virtual Reservation Reservation { get; set; } = null!;
    public virtual Language Language { get; set; } = null!;
}
