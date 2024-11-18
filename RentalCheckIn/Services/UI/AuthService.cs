using Fido2NetLib;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace RentalCheckIn.Services.UI;
public class AuthService : IAuthService
{
    private readonly HttpClient httpClient;
    private readonly ProtectedLocalStorage localStorage;
    private readonly IJSRuntime jSRuntime;
    private readonly NavigationManager navigationManager;
    private readonly AuthenticationStateProvider authStateProvider;
    private readonly RefreshTokenService refreshTokenService;
    private readonly IJWTService jWTService;
    private readonly ITOTPService tOTPService;

    public AuthService(HttpClient httpClient, ProtectedLocalStorage localStorage, IJSRuntime jSRuntime, NavigationManager navigationManager, AuthenticationStateProvider authStateProvider, RefreshTokenService refreshTokenService, IJWTService jWTService, ITOTPService tOTPService)
    {
        this.httpClient = httpClient;
        this.localStorage = localStorage;
        this.jSRuntime = jSRuntime;
        this.navigationManager = navigationManager;
        this.authStateProvider = authStateProvider;
        this.refreshTokenService = refreshTokenService;
        this.jWTService = jWTService;
        this.tOTPService = tOTPService;
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

    // TOTP Authentication 
    public async Task<OperationResult> VerifyTOTPAsync(TOTPDTO oTPModel)
    {
        try
        {
            LHost lHost = new LHost();
            // Retrieve the authenticated user
            var lHostResponseEntity = await httpClient.GetAsync($"api/LHost/email/{oTPModel.Email}");
            if (lHostResponseEntity.IsSuccessStatusCode)
            {

                if (lHostResponseEntity.StatusCode == HttpStatusCode.NoContent)
                {
                    return new OperationResult
                    {
                        IsSuccess = false,
                        Message = "User not found. Please register or contact support."
                    };

                }
                lHost = await lHostResponseEntity.Content.ReadFromJsonAsync<LHost>();
            }
            if (lHost == null)
            {
                return new OperationResult
                {
                    IsSuccess = false,
                    Message = "User not found. Please register or contact support."
                };
            }

            if (!tOTPService.VerifyCode(lHost.TotpSecret, oTPModel.Code))
                return new OperationResult { IsSuccess = false, Message = "Invalid OTP code." };

            var refreshToken = await refreshTokenService.GenerateRefreshToken(lHost.HostId);
            await localStorage.SetAsync("refreshToken", refreshToken.Token);
            var accessToken = jWTService.GenerateToken(lHost);
            Constants.JWTToken = accessToken;
            await localStorage.SetAsync("token", accessToken);

            await authStateProvider.GetAuthenticationStateAsync();

            navigationManager.NavigateTo("/");
            return new OperationResult { IsSuccess = true };
        }
        catch (Exception ex)
        {
            // Log the exception as needed
            return new OperationResult { IsSuccess = false, Message = "An unexpected error has occurred. Please try again later." };
        }
    }

    // FaceId Registration
    public async Task<OperationResult> RegisterFaceIdAsync(uint hostId)
    {
        try
        {
            // Fetch registration options from the server
            var optionsUrl = $"api/auth/faceid/register/options?hostId={hostId}";
            var options = await httpClient.GetFromJsonAsync<CredentialCreateOptions>(optionsUrl);

            if (options == null)
            {
                return new OperationResult
                {
                    IsSuccess = false,
                    Message = "Failed to retrieve registration options."
                };
            }

            // Perform WebAuthn registration using JavaScript
            var credential = await jSRuntime.InvokeAsync<object>("fido2.register", options);

            if (credential == null)
            {
                return new OperationResult
                {
                    IsSuccess = false,
                    Message = "Face ID registration was cancelled or failed."
                };
            }

            // Send registration response to the server
            var registerUrl = "api/auth/faceid/register";
            var response = await httpClient.PostAsJsonAsync(registerUrl, credential);

            if (response.IsSuccessStatusCode)
            {
                return new OperationResult
                {
                    IsSuccess = true,
                    Message = "Registration successful! Proceed to login."
                };
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
 
                return new OperationResult
                {
                    IsSuccess = false,
                    Message = $"Registration failed: {response.StatusCode} - {error}"
                };
            }
        }
        catch (Exception ex)
        {
            return new OperationResult
            {
                IsSuccess = false,
                Message = "Error during authentication, you might have taken taken too long before registering."
            };
        }
    }

    // Face ID Authentication
    public async Task<OperationResult> AuthenticateFaceIdAsync()
    {
        try
        {
            // Retrieve UserIdFor2FA from Protected Local Storage
            var userIdResult = await localStorage.GetAsync<uint>("UserIdFor2FA");
            if (!userIdResult.Success)
            {
                return new OperationResult
                {
                    IsSuccess = false,
                    Message = "Session expired. Please log in again."
                };
            }

            uint lHostId = userIdResult.Value;

            // Fetch authentication options from the server
            var optionsUrl = $"api/auth/faceid/authenticate/options?hostId={lHostId}";
            var options = await httpClient.GetFromJsonAsync<AssertionOptions>(optionsUrl);

            if (options == null)
            {
                return new OperationResult
                {
                    IsSuccess = false,
                    Message = "Failed to retrieve authentication options."
                };
            }

            // Perform WebAuthn authentication using JavaScript
            var assertion = await jSRuntime.InvokeAsync<object>("fido2.authenticate", options);

            if (assertion == null)
            {
                return new OperationResult
                {
                    IsSuccess = false,
                    Message = "Face ID authentication was cancelled or failed."
                };
            }

            // Send authentication response to the server
            var authenticateUrl = "api/auth/faceid/authenticate";
            var response = await httpClient.PostAsJsonAsync(authenticateUrl, assertion);

            if (response.IsSuccessStatusCode)
            {
                LHost lHost = new LHost();
                // Retrieve the authenticated user
                var lHostResponseEntity = await httpClient.GetAsync($"api/LHost/id/{lHostId}");
                if (lHostResponseEntity.IsSuccessStatusCode)
                {

                    if (lHostResponseEntity.StatusCode == HttpStatusCode.NoContent)
                    {
                        return new OperationResult
                        {
                            IsSuccess = false,
                            Message = "User not found. Please register or contact support."
                        };

                    }
                    lHost = await lHostResponseEntity.Content.ReadFromJsonAsync<LHost>();
                }
                if (lHost == null)
                {
                    return new OperationResult
                    {
                        IsSuccess = false,
                        Message = "User not found. Please register or contact support."
                    };
                }

                // Generate Refresh Token
                var refreshToken = await refreshTokenService.GenerateRefreshToken(lHost.HostId);
                await localStorage.SetAsync("refreshToken", refreshToken.Token);

                // Generate JWT Token
                var accessToken = jWTService.GenerateToken(lHost);
                Constants.JWTToken = accessToken; // Consider removing global constants for JWT
                await localStorage.SetAsync("token", accessToken);

                // Update Authentication State
                await authStateProvider.GetAuthenticationStateAsync();

                // Navigate to the home page or dashboard
                navigationManager.NavigateTo("/");

                return new OperationResult
                {
                    IsSuccess = true,
                    Message = "Authentication successful!"
                };
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();

                return new OperationResult
                {
                    IsSuccess = false,
                    Message = $"Authentication failed: {response.StatusCode} - {error}"
                };
            }
        }
        catch (Exception ex)
        {
            return new OperationResult
            {
                IsSuccess = false,
                Message = $"Error during authentication, you might have taken taken too long before authenticating."
            };
        }
    }
}


