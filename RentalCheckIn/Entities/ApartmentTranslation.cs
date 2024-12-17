using System.ComponentModel.DataAnnotations.Schema;

namespace RentalCheckIn.Entities;

public class ApartmentTranslation
{
    public uint ApartmentTranslationId { get; set; }
    public string ApartmentName { get; set; } = null!;
    public uint ApartmentId { get; set; }
    public uint LanguageId { get; set; }

    public virtual Apartment Apartment { get; set; } = null!;
    public virtual Language Language { get; set; } = null!;
}

