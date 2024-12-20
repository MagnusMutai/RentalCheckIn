using Fido2NetLib;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using RentalCheckIn.Locales;

namespace RentalCheckIn.Services.UI;
public class AuthUIService : IAuthUIService
{
    private readonly HttpClient httpClient;
    private readonly ProtectedLocalStorage localStorage;
    private readonly IJSRuntime jSRuntime;
    private readonly NavigationManager navigationManager;
    private readonly IRefreshTokenService refreshTokenBusinessService;
    private readonly IJWTService jWTService;
    private readonly ITOTPService tOTPService;
    private readonly ILogger<AuthUIService> logger;
    private readonly IStringLocalizer<Resource> localizer;

    // Too many injected services separate them into smaller services
    public AuthUIService(HttpClient httpClient, ProtectedLocalStorage localStorage, IJSRuntime jSRuntime, NavigationManager navigationManager, IRefreshTokenService refreshTokenBusinessService, IJWTService jWTService, ITOTPService tOTPService, ILogger<AuthUIService> logger, IStringLocalizer<Resource> localizer)
    {
        this.httpClient = httpClient;
        this.localStorage = localStorage;
        this.jSRuntime = jSRuntime;
        this.navigationManager = navigationManager;
        this.refreshTokenBusinessService = refreshTokenBusinessService;
        this.jWTService = jWTService;
        this.tOTPService = tOTPService;
        this.logger = logger;
        this.localizer = localizer;
    }

    public async Task<OperationResult<HostLoginResponseDTO>> LoginAsync(HostLoginDTO hostLoginDTO)
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

                return await response.Content.ReadFromJsonAsync<OperationResult<HostLoginResponseDTO>>();
            }

            return new OperationResult<HostLoginResponseDTO>
            {
                IsSuccess = false,
                Message = localizer["Error.TryAgainLater"]
            };

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred in AuthService while trying to login a LHost.");
            return new OperationResult<HostLoginResponseDTO>();
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

            return new OperationResult<LHost>
            {
                IsSuccess = false,
                Message = localizer["Error.TryAgainLater"]
            };

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred in AuthService while trying to register an.");

            return new OperationResult<LHost>
            {
                IsSuccess = false,
                Message = localizer["UnexpectedErrorOccurred"]
            };
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
                    Message = localizer["Error.Login.Failed"]
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
                    return new TokenValidateResult
                    {
                        IsSuccess = false,
                        Message = localizer["Error.Authorization.NotAuthorized"]
                    };
                }
                return await response.Content.ReadFromJsonAsync<TokenValidateResult>();
            }

            return new TokenValidateResult
            {
                IsSuccess = false,
                Message = localizer["Error.TryAgainLater"]
            };

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
            logger.LogError(ex, "An unexpected error has occurred in AuthService while trying to refresh tokens");

            return new TokenValidateResult
            {
                IsSuccess = false,
                Message = localizer["UnexpectedErrorOccurred"]
            };
        }

    }

    public async Task<OperationResult> ForgotPasswordAsync(ResetRequestDTO resetRequestDTO)
    {
        try
        {
            var json = JsonSerializer.Serialize(resetRequestDTO.Email);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("api/auth/forgot-password", content);

            return await response.Content.ReadFromJsonAsync<OperationResult>();

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error has occurred in AuthService while trying to send password reset request to the server.");

            return new OperationResult()
            {
                IsSuccess = false,
                Message = localizer["UnexpectedErrorOccurred"]
            };
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
                    return new EmailVerificationResponse
                    {
                        IsSuccess = false,
                        Message = localizer["Error.TryAgainLater"]
                    };
                }
                return await response.Content.ReadFromJsonAsync<EmailVerificationResponse>();
            }

            return new EmailVerificationResponse
            {
                IsSuccess = false,
                Message = localizer["Error.TryAgainLater"]
            };

        }
        catch (Exception ex)
        {
            return new EmailVerificationResponse
            {
                IsSuccess = false,
                Message = localizer["UnexpectedErrorOccurred"]
            };
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
                    return new OperationResult<string>
                    {
                        IsSuccess = false,
                        Message = localizer["Error.TryAgainLater"]
                    };
                }

                return await response.Content.ReadFromJsonAsync<OperationResult<string>>();
            }

            var message = await response.Content.ReadAsStringAsync();
            throw new Exception($"{message}");

        }
        catch (Exception ex)
        {
            return new OperationResult<string>
            {
                IsSuccess = false,
                Message = localizer["UnexpectedErrorOccurred"]
            };
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

            // We don't need the entire lHost.

            if (!tOTPService.VerifyCode(lHost.TOTPSecret, oTPModel.Code))
                return new OperationResult { IsSuccess = false, Message = "Invalid OTP code." };
            // Add null checks before assignments.
            var refreshToken = await refreshTokenBusinessService.GenerateRefreshToken(lHost.HostId);
            
            if (refreshToken != null)
            {
                await localStorage.SetAsync("refreshToken", refreshToken.Token);
            }

            var accessToken = jWTService.GenerateToken(lHost);

            if (accessToken != null) 
            { 
                Constants.JWTToken = accessToken;
                await localStorage.SetAsync("token", accessToken);
            }

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
                    Message = "Registration successful! An account confirmation link has been sent to your email click it and Proceed to login."
                };
            }
            else
            {
                // Log the error
                var error = await response.Content.ReadAsStringAsync();
 
                return new OperationResult
                {
                    IsSuccess = false,
                    Message = $"Registration failed. Please try again."
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
                navigationManager.NavigateTo("/login");
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
                var refreshToken = await refreshTokenBusinessService.GenerateRefreshToken(lHost.HostId);
                await localStorage.SetAsync("refreshToken", refreshToken.Token);
                // Generate JWT Token
                var accessToken = jWTService.GenerateToken(lHost);
                Constants.JWTToken = accessToken; // Consider removing global constants for JWT
                await localStorage.SetAsync("token", accessToken);
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


