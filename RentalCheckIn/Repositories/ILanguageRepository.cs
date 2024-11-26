namespace RentalCheckIn.Repositories;
public interface ILanguageRepository
{
    Task<Language?> GetLanguageByCultureAsync(string culture);
    Task<Language?> GetDefaultLanguageAsync();
}
