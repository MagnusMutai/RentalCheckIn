namespace RentalCheckIn.Services.Core;

public interface IReservationBusinessService
{
    Task<IEnumerable<ReservationDTO>> GetAllTableReservationsAsync();
    Task<IEnumerable<Setting>> GetSettingsAsync();
}
