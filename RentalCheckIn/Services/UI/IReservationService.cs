namespace RentalCheckIn.Services.UI;
public interface IReservationService
{
    Task<IEnumerable<ReservationDTO>> GetAllTableReservationsAsync();
    Task<CheckInFormDTO> GetCheckInFormReservationByIdAsync(uint reservationId);
    Task<IEnumerable<Setting>> GetSettingsAsync();
}
