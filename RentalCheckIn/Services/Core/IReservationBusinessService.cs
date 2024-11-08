namespace RentalCheckIn.Services.Core;

public interface IReservationBusinessService
{
    Task<IEnumerable<Reservation>> GetAllReservationsAsync();
}
