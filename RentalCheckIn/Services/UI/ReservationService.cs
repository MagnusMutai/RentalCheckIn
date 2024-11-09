

namespace RentalCheckIn.Services.UI;
public class ReservationService : IReservationService
{
    private readonly HttpClient httpClient;

    public ReservationService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<IEnumerable<ReservationDto>> GetAllReservationsAsync()
    {
        try
        {
            var response = await httpClient.GetAsync("api/reservation/AllReservations");
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return new List<ReservationDto>();
                }
                return await response.Content.ReadFromJsonAsync<IEnumerable<ReservationDto>>();
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

    public async Task<IEnumerable<Setting>> GetSettingsAsync()
    {
        try
        {
            var response = await httpClient.GetAsync("api/reservation/settings");
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return new List<Setting>();
                }
                return await response.Content.ReadFromJsonAsync<IEnumerable<Setting>>();
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
