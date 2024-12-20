namespace RentalCheckIn.Services.Core;

public interface IReservationService
{
    Task<IEnumerable<ReservationDTO>> GetAllTableReservationsAsync();
    Task<CheckInReservationDTO> GetCheckInReservationByIdAsync(uint reservationId);
    Task<OperationResult> UpdateCheckInReservationPartialAsync(CheckInReservationUpdateDTO checkInReservation);
    Task<IEnumerable<Setting>> GetSettingsAsync();
}
