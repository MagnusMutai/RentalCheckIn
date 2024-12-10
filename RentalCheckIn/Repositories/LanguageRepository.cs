
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

    public async Task<List<CultureFlagDTO>> GetAllLanguageFlagsAsync()
    {
        return await context.Languages
            .Select(l => new CultureFlagDTO
            {
                Id = l.LanguageId,
                FlagSvg = l.Svg
            }).ToListAsync();
            
    }
}
