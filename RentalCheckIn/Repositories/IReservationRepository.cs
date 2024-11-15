namespace RentalCheckIn.Repositories;

public interface IReservationRepository
{
    Task<IEnumerable<ReservationDTO>> GetAllReservationsAsync();
    Task<IEnumerable<Setting>> GetSettingsAsync();
}
