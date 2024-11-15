namespace RentalCheckIn.Services.Core;
public class ReservationBusinessService : IReservationBusinessService
{
    private readonly IReservationRepository reservationRepository;

    public ReservationBusinessService(IReservationRepository reservationRepository)
    {
        this.reservationRepository = reservationRepository;
    }

    public async Task<IEnumerable<ReservationDTO>> GetAllReservationsAsync()
    {
        try
        {
            return await reservationRepository.GetAllReservationsAsync();
        }
        catch (Exception ex)
        {
            // Return an empty list on error
            return Enumerable.Empty<ReservationDTO>(); 
        }
    }

    public async Task<IEnumerable<Setting>> GetSettingsAsync()
    {
        try
        {
            return await reservationRepository.GetSettingsAsync();
        }
        catch (Exception ex)
        {
            return Enumerable.Empty<Setting>();
        }
    }
}
