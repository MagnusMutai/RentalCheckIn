using System.Net.Http;

namespace RentalCheckIn.Services.UI;
public class LocalizationUIService : ILocalizationUIService
{
    private readonly HttpClient httpClient;

    public LocalizationUIService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<Dictionary<uint, string>> GetApartmentNamesAsync(IEnumerable<uint> apartmentIds, string culture)
    {
        var request = new ApartmentNamesRequest
        {
            ApartmentIds = apartmentIds.ToList(),
            Culture = culture
        };

        var response = await httpClient.PostAsJsonAsync("api/localization/apartments/names", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Dictionary<uint, string>>();
    }

    public async Task<Dictionary<uint, string>> GetStatusLabelsAsync(IEnumerable<uint> statusIds, string culture)
    {
        var request = new StatusLabelsRequest
        {
            StatusIds = statusIds.ToList(),
            Culture = culture
        };


        var response = await httpClient.PostAsJsonAsync("api/localization/statuses/labels", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Dictionary<uint, string>>();
    }
    public async Task<(string? CheckInTime, string? CheckOutTime)> GetReservationTimesAsync(uint reservationId)
    {
        var response = await httpClient.GetAsync($"api/localization/reservation/{reservationId}/times");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<(string?, string?)>();
    }
}
