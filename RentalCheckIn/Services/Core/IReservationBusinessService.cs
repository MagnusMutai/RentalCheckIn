namespace RentalCheckIn.Services.Core;

public interface IReservationBusinessService
{
    Task<IEnumerable<ReservationDto>> GetAllReservationsAsync();
    Task<IEnumerable<Setting>> GetSettingsAsync();
}
