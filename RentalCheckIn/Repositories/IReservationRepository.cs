namespace RentalCheckIn.Repositories
{
    public interface IReservationRepository
    {
        Task<IEnumerable<ReservationDto>> GetAllReservationsAsync();
    }
}
