namespace RentalCheckIn.Services.UI;
public interface IReservationService
{
    Task<IEnumerable<ReservationDTO>> GetAllReservationsAsync();
    Task<IEnumerable<Setting>> GetSettingsAsync();
}
