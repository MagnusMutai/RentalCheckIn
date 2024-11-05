using static RentalCheckIn.DTOs.CustomResponses;

namespace RentalCheckIn.Services.UI;

public class AuthService : IAuthService
{
    private readonly HttpClient httpClient;

    public AuthService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }
    public async Task<AuthenticationResponse> LoginAsync(HostLoginDto hostLoginDto)
    {
        try
        {

            var response = await httpClient.PostAsJsonAsync("api/auth/login", hostLoginDto);
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    return default(AuthenticationResponse);
                }
                return await response.Content.ReadFromJsonAsync<AuthenticationResponse>();
            }
            else
            {
                var message = await response.Content.ReadAsStringAsync();
                throw new Exception(message);
            }
        }
        catch (Exception)
        {
            // Log exception
            throw;
        }
    }

    public async Task<AuthenticationResponse> RegisterAsync(HostSignUpDto hostSignUpDto)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync("api/auth/register", hostSignUpDto);
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    return default(AuthenticationResponse);
                }
                return await response.Content.ReadFromJsonAsync<AuthenticationResponse>();
            }
            else
            {
                var message = await response.Content.ReadAsStringAsync();
                throw new Exception(message);
            }
        }
        catch (Exception)
        {
            // Log exception
            throw;
        }
    }

}
