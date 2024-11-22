namespace RentalCheckIn.Repositories;

public interface IReservationRepository
{
    Task<IEnumerable<ReservationDTO>> GetAllTableReservationsAsync();
    Task<CheckInReservationDTO?> GetCheckInReservationByIdAsync(uint reservationId);
    Task<Reservation?> GetReservationByIdAsync(uint reservationId);
    Task<IEnumerable<Setting>> GetSettingsAsync();
    Task<bool> UpdateCheckInReservationPartialAsync(Reservation reservation, Action<Reservation> patchData);
}
