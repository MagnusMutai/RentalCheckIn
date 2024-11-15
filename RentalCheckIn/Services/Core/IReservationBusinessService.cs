namespace RentalCheckIn.Services.Core;

public interface IReservationBusinessService
{
    Task<IEnumerable<ReservationDTO>> GetAllReservationsAsync();
    Task<IEnumerable<Setting>> GetSettingsAsync();
}
