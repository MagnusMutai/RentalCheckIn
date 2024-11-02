using RentalCheckIn.Dtos;

namespace RentalCheckIn.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient httpClient;

    public AuthService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }
    public async Task<Lhost> LoginAsync(HostLoginDto hostLoginDto)
    {
        var response = await httpClient.PostAsJsonAsync("api/auth/login", hostLoginDto);
        var result = await response.Content.ReadFromJsonAsync<Lhost>();
        return result;
    }

    public async Task<Lhost> RegisterAsync(HostSignUpDto hostSignUpDto)
    {
        var response = await httpClient.PostAsJsonAsync("api/auth/register", hostSignUpDto);
        var result = await response.Content.ReadFromJsonAsync<Lhost>();
        return result;
    }
}
