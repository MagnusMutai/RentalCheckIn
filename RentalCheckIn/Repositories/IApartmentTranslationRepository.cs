namespace RentalCheckIn.Repositories;
public interface IApartmentTranslationRepository
{
    Task<IEnumerable<ApartmentTranslation>> GetTranslationsAsync(IEnumerable<uint> apartmentIds, uint languageId);
}
