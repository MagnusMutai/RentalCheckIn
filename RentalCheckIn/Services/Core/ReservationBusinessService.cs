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

    public async Task<CheckInReservationDTO> GetCheckInReservationByIdAsync(uint reservationId)
    {
        try
        {
            return await reservationRepository.GetCheckInReservationByIdAsync(reservationId);
        }
        catch (Exception ex) 
        {
            return new CheckInReservationDTO();
        }
    }
    public async Task UpdateCheckInReservationAsync(CheckInReservationUpdateDTO dto)
    {
        var reservation = await reservationRepository.GetCheckInReservationByIdAsync(dto.Id);
        if (reservation == null)
        {
            throw new Exception("Reservation not found");
        }
        

        // Save changes
        await reservationRepository.UpdateCheckInReservationAsync(reservation);
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
