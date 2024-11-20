namespace RentalCheckIn.Repositories;

public class ReservationRepository : IReservationRepository
{
    private readonly AppDbContext context;

    public ReservationRepository(AppDbContext context)
    {
        this.context = context;
    }
    public async Task<IEnumerable<ReservationDTO>> GetAllTableReservationsAsync()
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
                .Select(r => new ReservationDTO
                {
                    Id = r.ReservationId,
                    CheckInDate = r.CheckInDate,
                    CheckOutDate = r.CheckOutDate,
                    CheckInTime = r.CheckInTime,
                    CheckOutTime = r.CheckOutTime,
                    QuestName = r.Quest.FirstName,
                    PhoneNumber = r.Quest.Mobile,
                    NumberOfQuests = r.NumberOfQuests,
                    NumberOfNights = r.NumberOfNights,
                    Price = r.ApartmentFee,
                    SecurityDeposit = r.SecurityDeposit,
                    TotalPrice = r.TotalPrice,
                    ApartmentName = r.Apartment.ApartmentName,
                    ChannelName = r.Channel.ChannelLabel,
                    CurrencySymbol = r.Currency.CurrencySymbol,
                    StatusLabel = r.Status.StatusLabel
                }).ToListAsync();
    }

    public async Task<CheckInFormDTO?> GetCheckInFormReservationDataByIdAsync(uint reservationId)
    {
        return await context.Reservations
            .Include(r => r.Apartment)
            .Include(r => r.Currency)
            .Include(r => r.Quest)
            .Where(r => r.ReservationId == reservationId)
                .Select(r => new CheckInFormDTO
                {
                    GuestName = r.Quest.FirstName,
                    PassportNr = r.Quest.PassportNr,
                    MailAddress = r.Quest.MailAddress,
                    Mobile = r.Quest.Mobile,
                    CheckInDate = r.CheckInDate,
                    CheckOutDate = r.CheckOutDate,
                    CheckInTime = r.CheckInTime,
                    CheckOutTime = r.CheckOutTime,
                    NumberOfNights = r.NumberOfNights,
                    NumberOfGuests = r.NumberOfQuests,
                    ApartmentName = r.Apartment.ApartmentName,
                    ApartmentFee = r.ApartmentFee,
                    SecurityDeposit = r.SecurityDeposit,
                    TotalPrice = r.TotalPrice,
                    KwhAtCheckIn = r.KwhAtCheckIn,
                    SignatureDataUrl = r.SignatureQuest,
                    CurrencySymbol = r.Currency.CurrencySymbol
                }).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Setting>> GetSettingsAsync()
    {
        return await context.Settings.ToListAsync();
    }
}
