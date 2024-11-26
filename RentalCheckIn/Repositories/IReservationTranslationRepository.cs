namespace RentalCheckIn.Repositories;
public interface IReservationTranslationRepository
{
    Task<ReservationTranslation?> GetTranslationAsync(uint reservationId, uint languageId);
}
