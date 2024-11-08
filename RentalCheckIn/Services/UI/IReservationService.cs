namespace RentalCheckIn.Services.UI;
public interface IReservationService
{
    Task<IEnumerable<Reservation>> GetAllReservationsAsync();
}
