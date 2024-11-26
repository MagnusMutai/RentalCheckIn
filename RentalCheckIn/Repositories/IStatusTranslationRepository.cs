namespace RentalCheckIn.Repositories;
public interface IStatusTranslationRepository
{
    Task<IEnumerable<StatusTranslation>> GetTranslationsAsync(IEnumerable<uint> statusIds, uint languageId);
}
