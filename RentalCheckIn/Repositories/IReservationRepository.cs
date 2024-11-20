namespace RentalCheckIn.Repositories;

public interface IReservationRepository
{
    Task<IEnumerable<ReservationDTO>> GetAllTableReservationsAsync();
    Task<CheckInFormDTO> GetCheckInFormReservationDataByIdAsync(uint reservationId);
    Task<IEnumerable<Setting>> GetSettingsAsync();
}
