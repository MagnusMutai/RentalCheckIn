namespace RentalCheckIn.Entities;

public partial class Country
{
    public uint CountryId { get; set; }
    public string CountryName { get; set; } = null!;
    public string CountryIso2 { get; set; } = null!;
    public string? Nationality { get; set; }
    public bool IsEnabled { get; set; }
    public string? Dial { get; set; }
    public DateTime CreationDate { get; set; }
    // There's no navigation property for the Language entity
    public uint? LanguageId { get; set; }

    public virtual ICollection<Quest> Quests { get; set; } = new List<Quest>();
}
