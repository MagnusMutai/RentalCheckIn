namespace RentalCheckIn.Repositories;

public interface IReservationRepository
{
    Task<IEnumerable<ReservationDTO>> GetAllTableReservationsAsync();
    Task<IEnumerable<Setting>> GetSettingsAsync();
}
