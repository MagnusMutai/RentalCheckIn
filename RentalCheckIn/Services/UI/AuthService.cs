using static System.Net.WebRequestMethods;

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

        try
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

            async Task<string> RetrieveToken(string key)
            {
                // Retrieve token from local storage
                var response = await localStorage.GetAsync<string>(key);
                return response.Value;
            }
        }
        catch(Exception) 
        {
            throw;
        }
    }

    public async Task<ResetPasswordResponse> ForgotPassword(PasswordResetDto PasswordResetDto)
    {
        try
        {
            var json = JsonSerializer.Serialize(PasswordResetDto.Email);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync($"api/auth/forgot-password", content);

            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NoContent) 
                {
                    return default;
                }
                return await response.Content.ReadFromJsonAsync<ResetPasswordResponse>();
            }
            else
            {
                var message = await response.Content.ReadAsStringAsync();
                throw new Exception(message);
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<EmailVerificationResponse> VerifyEmailAsync(string eleVerificationToken)
    {
        try
        {
            // Call the API endpoint to verify the email token
            var response = await httpClient.PostAsJsonAsync("api/Auth/verify-email", eleVerificationToken);

            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    return default;
                }
               return await response.Content.ReadFromJsonAsync<EmailVerificationResponse>();
            }
            else
            {
                var message = await response.Content.ReadAsStringAsync();
                throw new Exception(message);
            }
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}


