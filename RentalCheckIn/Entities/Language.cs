namespace RentalCheckIn.Entities;

public partial class Language
{
    public uint LanguageId { get; set; }
    public string LanguageCode { get; set; } = null!;
    public string LanguageName { get; set; } = null!;
    public string Culture { get; set; } = null!;
    public bool IsEnabled { get; set; }
    public string Svg { get; set; } = default!;
    public DateTime CreationDate { get; set; }
    public virtual ICollection<Quest> Quests { get; set; } = new List<Quest>();
    public virtual ICollection<ApartmentTranslation> ApartmentTranslations { get; set; } = new List<ApartmentTranslation>();
    public virtual ICollection<StatusTranslation> StatusTranslations { get; set; } = new List<StatusTranslation>();
    public virtual ICollection<ReservationTranslation> ReservationTranslations { get; set; } = new List<ReservationTranslation>();
}
