namespace RentalCheckIn.Services.UI;

public class LHostService : ILHostService
{
    private readonly HttpClient httpClient;

    public LHostService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }
    public async Task<LHost> GetLHostByEmail(string mailAddress)
    {
        try
        {
            var response = await httpClient.GetAsync($"api/LHost/email/{mailAddress}");
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    return default(LHost);
                }
                return await response.Content.ReadFromJsonAsync<LHost>();
            }
            else
            {
                var message = await response.Content.ReadAsStringAsync();
                throw new Exception(message);
            }
        }

        catch (Exception)
        {
            // Don't throw in production
            // Log exception
            throw;
        }
    }

    public async Task<LHost> GetLHostById(uint lHostId)
    {
        try
        {
            var response = await httpClient.GetAsync($"api/LHost/id/{lHostId}");
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    return default(LHost);
                }
                return await response.Content.ReadFromJsonAsync<LHost>();
            }
            else
            {
                var message = await response.Content.ReadAsStringAsync();
                throw new Exception(message);
            }
        }

        catch (Exception)
        {
            // Don't throw in production
            // Log exception
            throw;
        }
    }
}
