namespace RentalCheckIn.Repositories;

public interface IReservationRepository
{
    Task<IEnumerable<ReservationDTO>> GetAllTableReservationsAsync();
    Task<CheckInReservationDTO> GetCheckInReservationByIdAsync(uint reservationId);
    Task<IEnumerable<Setting>> GetSettingsAsync();
    Task UpdateCheckInReservationAsync(Reservation reservation);
}
