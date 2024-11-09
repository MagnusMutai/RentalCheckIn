namespace RentalCheckIn.Services.UI;
public interface IReservationService
{
    Task<IEnumerable<ReservationDto>> GetAllReservationsAsync();
    Task<IEnumerable<Setting>> GetSettingsAsync();
}
