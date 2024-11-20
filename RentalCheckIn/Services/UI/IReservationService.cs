namespace RentalCheckIn.Services.UI;
public interface IReservationService
{
    Task<IEnumerable<ReservationDTO>> GetAllTableReservationsAsync();
    Task<IEnumerable<CheckInFormDTO>> GetCheckInFormReservationDataByIdAsync(uint reservationId);
    Task<IEnumerable<Setting>> GetSettingsAsync();
}
