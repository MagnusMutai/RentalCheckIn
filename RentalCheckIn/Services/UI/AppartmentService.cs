namespace RentalCheckIn.Services.UI;

public class AppartmentService : IAppartmentService
{
    private readonly HttpClient httpClient;

    public AppartmentService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }
    public async Task<IEnumerable<string>> GetDistinctAppartmentNames()
    {
        try
        {

            var response = await httpClient.GetAsync("api/Appartment");
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    return default;
                }
                return await response.Content.ReadFromJsonAsync<List<string>>();
            }
            else
            {
                var message = await response.Content.ReadAsStringAsync();
                throw new Exception(message);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return new List<string>();
        }
    }
}
