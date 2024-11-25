namespace RentalCheckIn.Entities;

public partial class Status
{
    public uint StatusId { get; set; }
    public string StatusLabel { get; set; } = null!;
    public DateTime CreationDate { get; set; }

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    public virtual ICollection<StatusTranslation> StatusTranslations { get; set; } = new List<StatusTranslation>();
}
