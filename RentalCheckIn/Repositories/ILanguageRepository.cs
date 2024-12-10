namespace RentalCheckIn.Repositories;
public interface ILanguageRepository
{
    Task<List<CultureFlagDTO>> GetAllLanguageFlagsAsync();
    Task<Language?> GetLanguageByCultureAsync(string culture);
    Task<Language?> GetDefaultLanguageAsync();
}
