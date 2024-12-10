namespace RentalCheckIn.Services.UI;
public class LocalizationUIService : ILocalizationUIService
{
    private readonly HttpClient httpClient;

    public LocalizationUIService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<List<CultureFlagDTO>> GetAllLanguageFlagsAsync()
    {
        try
        {
            var response = await httpClient.GetAsync("api/localization/languages/flags");
            
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    return new List<CultureFlagDTO>();
                }

                return await response.Content.ReadFromJsonAsync<List<CultureFlagDTO>>() ?? new List<CultureFlagDTO>();
            }
            // Return an empty container and log exception
            return new List<CultureFlagDTO>();
        }
        catch (Exception ex)
        {
            // Log error and return an empty List of string, Eventually we could use Result pattern.
            return new List<CultureFlagDTO>();
        }
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

        return await response.Content.ReadFromJsonAsync<Dictionary<uint, string>>() ?? new Dictionary<uint, string>();
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
        return await response.Content.ReadFromJsonAsync<Dictionary<uint, string>>() ?? new Dictionary<uint, string>();
    }
}
