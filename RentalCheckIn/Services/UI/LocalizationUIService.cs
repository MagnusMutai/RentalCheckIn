using System.Net.Http;
using static RentalCheckIn.DTOs.CustomRequests;

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
        var request = new LocalizationRequest<uint>
        {
            Data = apartmentIds.ToList(),
            Culture = culture
        };

        var response = await httpClient.PostAsJsonAsync("api/localization/apartments/names", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Dictionary<uint, string>>();
    }

    public async Task<Dictionary<uint, string>> GetStatusLabelsAsync(IEnumerable<uint> statusIds, string culture)
    {
        var request = new LocalizationRequest<uint>
        {
            Data = statusIds.ToList(),
            Culture = culture
        };


        var response = await httpClient.PostAsJsonAsync("api/localization/statuses/labels", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Dictionary<uint, string>>();
    }
}
