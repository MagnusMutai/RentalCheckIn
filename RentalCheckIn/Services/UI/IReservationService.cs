namespace RentalCheckIn.Services.UI;
public interface IReservationService
{
    Task<IEnumerable<ReservationDto>> GetAllReservationsAsync();
}
