using RentalCheckIn.Entities;
using System.Text.Json;

namespace RentalCheckIn.Services.UI;

public class AuthService : IAuthService
{
    private readonly HttpClient httpClient;
    private readonly ProtectedLocalStorage localStorage;

    public AuthService(HttpClient httpClient, ProtectedLocalStorage localStorage)
    {
        this.httpClient = httpClient;
        this.localStorage = localStorage;
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

    public async Task RefreshTokenAsync()
    {
        var accessToken = await RetrieveToken("token");
        var refreshToken = await RetrieveToken("refreshToken");


        var data = new
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };

        var json = JsonSerializer.Serialize(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync("api/auth/refresh-token", content);
    }

    private async Task<string> RetrieveToken(string key)
    {
        // Retrieve token from local storage
        var response = await localStorage.GetAsync<string>(key);
        return response.Value;
    }
}


