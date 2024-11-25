namespace RentalCheckIn.Entities;

public class StatusTranslation
{
    public int StatusTranslationId { get; set; }
    public uint StatusId { get; set; }
    public uint LanguageId { get; set; }
    public string StatusLabel { get; set; } = null!;

    public virtual Status Status { get; set; } = null!;
    public virtual Language Language { get; set; } = null!;
}
