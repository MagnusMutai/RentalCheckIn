using System.ComponentModel.DataAnnotations.Schema;

namespace RentalCheckIn.Entities;

public class ApartmentTranslation
{
    public int ApartmentTranslationId { get; set; }
    public uint ApartmentId { get; set; }
    public uint LanguageId { get; set; }
    public string ApartmentName { get; set; } = null!;

    public virtual Apartment Apartment { get; set; } = null!;
    public virtual Language Language { get; set; } = null!;
}

