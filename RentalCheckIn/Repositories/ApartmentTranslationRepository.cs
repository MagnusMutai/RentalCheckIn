using Microsoft.EntityFrameworkCore;

namespace RentalCheckIn.Repositories;
public class ApartmentTranslationRepository : IApartmentTranslationRepository
{
    private readonly AppDbContext context;

    public ApartmentTranslationRepository(AppDbContext context)
    {
        this.context = context;
    }

    public async Task<IEnumerable<ApartmentTranslation>> GetTranslationsAsync(IEnumerable<uint> apartmentIds, uint languageId)
    {
        return await context.ApartmentTranslations
            .Where(at => apartmentIds.Contains(at.ApartmentId) && at.LanguageId == languageId)
            .ToListAsync();
    }

}
