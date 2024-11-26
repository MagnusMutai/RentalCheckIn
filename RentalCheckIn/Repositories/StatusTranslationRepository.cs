namespace RentalCheckIn.Repositories;
public class StatusTranslationRepository : IStatusTranslationRepository
{
    private readonly AppDbContext context;

    public StatusTranslationRepository(AppDbContext context)
    {
        this.context = context;
    }

    public async Task<IEnumerable<StatusTranslation>> GetTranslationsAsync(IEnumerable<uint> statusIds, uint languageId)
    {
        return await context.StatusTranslations
            .Where(st => statusIds.Contains(st.StatusId) && st.LanguageId == languageId)
            .ToListAsync();
    }

}
