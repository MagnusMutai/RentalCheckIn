namespace RentalCheckIn.Services.Core;

public interface IReservationBusinessService
{
    Task<IEnumerable<ReservationDTO>> GetAllTableReservationsAsync();
    Task<CheckInFormDTO> GetCheckInFormReservationByIdAsync(uint reservationId);
    Task<IEnumerable<Setting>> GetSettingsAsync();
}
