namespace RentalCheckIn.Repositories;
public class ReservationTranslationRepository : IReservationTranslationRepository
{
    private readonly AppDbContext context;

    public ReservationTranslationRepository(AppDbContext context)
    {
        this.context = context;
    }

    public async Task<ReservationTranslation?> GetTranslationAsync(uint reservationId, uint languageId)
    {
        return await context.ReservationTranslations
                             .FirstOrDefaultAsync(rt => rt.ReservationId == reservationId && rt.LanguageId == languageId);
    }
}
