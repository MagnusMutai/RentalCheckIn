namespace RentalCheckIn.Services.Core;
public class ReservationBusinessService : IReservationBusinessService
{
    private readonly IReservationRepository reservationRepository;

    public ReservationBusinessService(IReservationRepository reservationRepository)
    {
        this.reservationRepository = reservationRepository;
    }

    public async Task<IEnumerable<ReservationDTO>> GetAllTableReservationsAsync()
    {
        try
        {
            return await reservationRepository.GetAllTableReservationsAsync();
        }
        catch (Exception ex)
        {
            // Return an empty list on error
            return Enumerable.Empty<ReservationDTO>(); 
        }
    }

    public async Task<CheckInFormDTO> GetCheckInFormReservationByIdAsync(uint reservationId)
    {
        try
        {
            return await reservationRepository.GetCheckInFormReservationByIdAsync(reservationId);
        }
        catch (Exception ex) 
        {
            return new CheckInFormDTO();
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
