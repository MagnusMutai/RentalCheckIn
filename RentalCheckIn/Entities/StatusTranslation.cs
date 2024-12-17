namespace RentalCheckIn.Entities;

public class StatusTranslation
{
    public uint StatusTranslationId { get; set; }
    public string StatusLabel { get; set; } = null!;
    public uint StatusId { get; set; }
    public uint LanguageId { get; set; }
    public virtual Status Status { get; set; } = null!;
    public virtual Language Language { get; set; } = null!;
}
