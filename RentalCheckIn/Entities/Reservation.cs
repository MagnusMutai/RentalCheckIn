namespace RentalCheckIn.Entities;

public partial class Reservation
{
    public uint ReservationId { get; set; }
    public DateOnly CheckInDate { get; set; }
    public string? CheckInTime { get; set; }
    public DateOnly CheckOutDate { get; set; }
    public string? CheckOutTime { get; set; }
    // Current actual date
    public DateTime? CheckedInAt { get; set; }
    // Current actual date
    public DateTime? CheckedOutAt { get; set; }
    public int NumberOfQuests { get; set; }
    public int NumberOfNights { get; set; }
    public decimal ApartmentFee { get; set; }
    public decimal SecurityDeposit { get; set; }
    public decimal? TotalPrice { get; set; }
    public int KwhAtCheckIn { get; set; }
    public int? KwhAtCheckOut { get; set; }
    public int KwhPerNightIncluded { get; set; }
    public decimal? CostsPerXtraKwh { get; set; }
    public bool AgreeEnergyConsumption { get; set; } //CheckBoxes
    public bool ReceivedKeys { get; set; }
    public bool AgreeTerms { get; set; }
    public uint CurrencyId { get; set; }
    public string? SignatureQuest { get; set; }
    public uint ApartmentId { get; set; }
    public uint QuestId { get; set; }
    public uint ChannelId { get; set; }
    public uint? HostId { get; set; }
    public uint StatusId { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public DateTime? CreationDate { get; set; }
    public virtual Apartment Apartment { get; set; } = null!;
    public virtual Channel Channel { get; set; } = null!;
    public virtual Currency Currency { get; set; } = null!;
    public virtual LHost? Host { get; set; }
    public virtual Quest Quest { get; set; } = null!;
    public virtual Status? Status { get; set; }
    public virtual ICollection<ReservationTranslation> ReservationTranslations { get; set; } = new List<ReservationTranslation>();
}
