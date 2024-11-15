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
    public async Task<OperationResult<LHost>> LoginAsync(HostLoginDTO hostLoginDTO)
    {
        try
        {

            var response = await httpClient.PostAsJsonAsync("api/auth/login", hostLoginDTO);
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    return default;
                }
                return await response.Content.ReadFromJsonAsync<OperationResult<LHost>>();
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
            return new OperationResult<LHost>();
        }

    }

    public async Task<OperationResult<LHost>> RegisterAsync(HostSignUpDTO hostSignUpDTO)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync("api/auth/register", hostSignUpDTO);
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    return default;
                }
                return await response.Content.ReadFromJsonAsync<OperationResult<LHost>>();
            }
            else
            {
                var message = await response.Content.ReadAsStringAsync();
                throw new Exception(message);
            }
        }
        catch (Exception ex)
        {
            // Log exception
            Console.WriteLine(ex.Message);
            return new OperationResult<LHost>();
        }

    }

    public async Task<TokenValidateResult> RefreshTokenAsync()
    {

        try
        {
            var accessToken = await RetrieveToken("token");
            var refreshToken = await RetrieveToken("refreshToken");

            if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
            {
                return new TokenValidateResult
                {
                    IsSuccess = false,
                    Message = "Cannot send empty tokens to the server."
                };
            }
            var data = new
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("api/auth/refresh-token", content);
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    return default(TokenValidateResult);
                }
                return await response.Content.ReadFromJsonAsync<TokenValidateResult>();
            }
            else
            {
                var message = await response.Content.ReadAsStringAsync();
                throw new Exception(message);
            }
            // Put it outside for single responsibility
            async Task<string> RetrieveToken(string key)
            {
                // Retrieve token from local storage
                var result = await localStorage.GetAsync<string>(key);
                return result.Value;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return new TokenValidateResult();
        }

    }

    public async Task<OperationResult> ForgotPasswordAsync(ResetRequestDTO resetRequestDTO)
    {
        try
        {
            var json = JsonSerializer.Serialize(resetRequestDTO.Email);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync($"api/auth/forgot-password", content);

            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    return default;
                }
                return await response.Content.ReadFromJsonAsync<OperationResult>();
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
            return new OperationResult();
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
            Console.WriteLine(ex.Message);
            return new EmailVerificationResponse();
        }
    }
    public async Task<OperationResult<string>> ResetPasswordAsync(string token, PasswordResetDTO passwordResetDTO)
    {
        try
        {
            var payload = new PasswordResetRequest
            {
                ResetToken = token,
                NewPassword = passwordResetDTO.NewPassword,
            };

            // Send the new password and token to the backend
            var response = await httpClient.PostAsJsonAsync("api/auth/reset-password", payload);

            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    return default;
                }
                return await response.Content.ReadFromJsonAsync<OperationResult<string>>();
            }
            else
            {
                var message = await response.Content.ReadAsStringAsync();
                throw new Exception($"{message}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return new OperationResult<string>();
        }
    }
}


