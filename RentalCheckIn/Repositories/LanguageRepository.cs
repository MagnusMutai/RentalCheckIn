namespace RentalCheckIn.Repositories;
public class LanguageRepository : ILanguageRepository
{
    private readonly AppDbContext context;
    private readonly string defaultLanguageCode = "en-EN";

    public LanguageRepository(AppDbContext context)
    {
        this.context = context;
    }

    public async Task<Language?> GetLanguageByCultureAsync(string culture)
    {
        return await context.Languages.FirstOrDefaultAsync(l => l.Culture == culture);
    }

    public async Task<Language?> GetDefaultLanguageAsync()
    {
        return await GetLanguageByCultureAsync(defaultLanguageCode);
    }
}
