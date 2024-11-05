using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using RentalCheckIn.Services.UI;
using RentalCheckIn.Utilities;
using System.Security.Cryptography;

namespace RentalCheckIn.Services.Core;

public class RefreshTokenService
{
    private readonly IRefreshTokenRepository refreshTokenRepository;
    private readonly ProtectedLocalStorage localStorage;
    private readonly HttpClient httpClient;

    public RefreshTokenService(IRefreshTokenRepository refreshTokenRepository, ProtectedLocalStorage localStorage, HttpClient httpClient)
    {
        this.refreshTokenRepository = refreshTokenRepository;
        this.localStorage = localStorage;
        this.httpClient = httpClient;
    }

    public async Task<RefreshToken> GenerateRefreshToken(uint lHostId)
    {

        // Revoke any existing active refresh tokens for the host
        var existingTokens = await refreshTokenRepository.GetActiveRefreshTokensByHostIdAsync(lHostId);
        foreach (var token in existingTokens)
        {
            token.IsRevoked = true; // Mark each as revoked
        }

        // **Add Error Handling
        var newRefreshToken = new RefreshToken
        {
            Token = GenerateSecureToken(),
            // Set refresh token lifespan
            Expires = DateTime.UtcNow.AddDays(7),
            Created = DateTime.UtcNow,
            IsRevoked = false,
            HostId = lHostId
        };

        // Save revoked tokens and the new token to the database
        await refreshTokenRepository.RevokeAndAddRefreshTokenAsync(existingTokens, newRefreshToken);


        // Store the token in the database
        //await refreshTokenRepository.AddRefreshTokenAsync(refreshToken);


        return newRefreshToken;
    }

    private string GenerateSecureToken()
    {
        var randomNumber = new byte[64];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }

    public async Task ValidateAndRefreshTokensAsync()
    {
        var accessToken = await RetrieveToken("token");
        var refreshToken = await RetrieveToken("refreshToken");

        if (!string.IsNullOrEmpty(accessToken) && !Extensions.IsTokenExpired(accessToken)) 
        {
            Constants.JWTToken = accessToken;
        }

        if (string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(accessToken))
        {
            Console.WriteLine("No tokens found, redirecting to login...");
            // Redirect to login 
            return;
        }

        // Check if the access token is expired
        if (Extensions.IsTokenExpired(accessToken))
        {
            // Wrap the refreshToken string in StringContent and format it as JSON
            var requestContent = new StringContent($"\"{refreshToken}\"", Encoding.UTF8, "application/json");

            // Call backend to refresh tokens if refresh token is valid
            var response = await httpClient.PostAsync("/api/auth/refresh-token", requestContent);

            if (response.IsSuccessStatusCode)   
            {
                var responseContent = await response.Content.ReadFromJsonAsync<TokenResponse>();
                if (responseContent != null)
                {
                    StoreToken("token", responseContent.NewAccessToken);
                    StoreToken("refreshToken", responseContent.NewRefreshToken);
                    Console.WriteLine("Tokens refreshed successfully.");
                }
            }
            else
            {
                Console.WriteLine("Refresh token invalid or expired, redirecting to login...");
                // Redirect to login or show login UI
            }
        }
    }

    private async Task<string> RetrieveToken(string key)
    {
        // Retrieve token from local storage 
        var response = await localStorage.GetAsync<string>(key);
        return response.Value;
    }

    private void StoreToken(string key, string token)
    {
        // Store token securely in local storage 
        localStorage.SetAsync(key, token);
    }
}

public class TokenResponse
{
    public string NewAccessToken { get; set; }
    public string NewRefreshToken { get; set; }
}

