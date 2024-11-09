namespace RentalCheckIn.Services.Core;

public interface IReservationBusinessService
{
    Task<IEnumerable<ReservationDto>> GetAllReservationsAsync();
}
