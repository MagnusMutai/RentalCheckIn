namespace RentalCheckIn.Services.UI;
public interface IReservationService
{
    Task<IEnumerable<ReservationDTO>> GetAllTableReservationsAsync();
    Task<CheckInReservationDTO> GetCheckInReservationByIdAsync(uint reservationId);
    Task<bool> UpdateCheckInFormReservationAsync(CheckInReservationDTO checkInModel);
    Task<IEnumerable<Setting>> GetSettingsAsync();
}
