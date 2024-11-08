
namespace RentalCheckIn.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly AppDbContext context;

        public ReservationRepository(AppDbContext context)
        {
            this.context = context;
        }
        public async Task<IEnumerable<Reservation>> GetAllReservationsAsync()
        {
            return await context.Reservations
                .Include(r => r.Apartment)
                .Include(r => r.Channel)
                .Include(r => r.Currency)
                .Include(r => r.Host)
                .Include(r => r.Quest)
                .Include(r => r.Status)
                .ToListAsync();
        }
    }
}
