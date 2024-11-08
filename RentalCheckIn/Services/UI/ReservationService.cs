
namespace RentalCheckIn.Services.UI;
public class ReservationService : IReservationService
{
    private readonly HttpClient httpClient;

    public ReservationService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<IEnumerable<Reservation>> GetAllReservationsAsync()
    {
        try
        {
            var response = await httpClient.GetAsync("api/reservation");
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return new List<Reservation>();
                }
                return await response.Content.ReadFromJsonAsync<IEnumerable<Reservation>>();
            }
            else
            {
                var message = await response.Content.ReadAsStringAsync();
                throw new Exception($"{message}");
            }
        }
        catch (Exception ex)
        {
            throw;
        }

    }
}
