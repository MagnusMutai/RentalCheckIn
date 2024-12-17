namespace RentalCheckIn.Entities;

public partial class Quest
{
    public uint QuestId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? Mobile { get; set; }
    public string? MailAddress { get; set; }
    public string? PassportNr { get; set; }
    public DateTime CreationDate { get; set; }
    public uint? LanguageId { get; set; }
    public uint? CountryId { get; set; }
    public virtual Country? Country { get; set; }
    public virtual Language? Language { get; set; }
    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
