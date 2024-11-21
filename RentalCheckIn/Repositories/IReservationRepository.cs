namespace RentalCheckIn.Repositories;

public interface IReservationRepository
{
    Task<IEnumerable<ReservationDTO>> GetAllTableReservationsAsync();
    Task<CheckInFormDTO> GetCheckInFormReservationByIdAsync(uint reservationId);
    Task<IEnumerable<Setting>> GetSettingsAsync();
}
