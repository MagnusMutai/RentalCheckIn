

namespace RentalCheckIn.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly AppDbContext context;

        public ReservationRepository(AppDbContext context)
        {
            this.context = context;
        }
        public async Task<IEnumerable<ReservationDto>> GetAllReservationsAsync()
        {
            return await context.Reservations
                .Include(r => r.Apartment)
                .Include(r => r.Channel)
                .Include(r => r.Currency)
                .Include(r => r.Host)
                .Include(r => r.Quest)
                .Include(r => r.Status)
                .Where(r => r.StatusId < 3)
                .OrderBy(r => r.CheckInDate)
                    .Select(r => new ReservationDto
                    {
                        CheckInDate = r.CheckInDate,
                        CheckOutDate = r.CheckOutDate,
                        QuestName = r.Quest.FirstName,
                        NumberOfQuests = r.NumberOfQuests,
                        NumberOfNights = r.NumberOfNights,
                        TotalPrice = r.TotalPrice,
                        ApartmentName = r.Apartment.ApartmentName,
                        ChannelName = r.Channel.ChannelLabel,
                        CurrencySymbol = r.Currency.CurrencySymbol,
                        StatusLabel = r.Status.StatusLabel
                        // Map other fields as needed
                    }).ToListAsync();
        }

        public async Task<IEnumerable<Setting>> GetSettingsAsync()
        {
            return await context.Settings.ToListAsync();
        }
    }
}
