using RentalCheckIn.Dtos;
using static RentalCheckIn.Responses.CustomResponses;

namespace RentalCheckIn.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient httpClient;

    public AuthService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }
    public async Task<AuthenticationResult> LoginAsync(HostLoginDto hostLoginDto)
    {
        var response = await httpClient.PostAsJsonAsync("api/auth/login", hostLoginDto);
        var result = await response.Content.ReadFromJsonAsync<AuthenticationResult>();
        return result;
    }

    public async Task<AuthenticationResult> RegisterAsync(HostSignUpDto hostSignUpDto)
    {
        var response = await httpClient.PostAsJsonAsync("api/auth/register", hostSignUpDto);
        var result = await response.Content.ReadFromJsonAsync<AuthenticationResult>();
        return result;
    }
}
