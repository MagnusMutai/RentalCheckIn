using System.Linq;

namespace RentalCheckIn.Services.Core;

public class RefreshTokenService
{
    private readonly IRefreshTokenRepository refreshTokenRepository;
    private readonly ProtectedLocalStorage localStorage;
    private readonly HttpClient httpClient;
    private readonly IAccountService accountService;
    private readonly IJwtService jwtService;
    private readonly AuthenticationStateProvider authStateProvider;

    public RefreshTokenService(IRefreshTokenRepository refreshTokenRepository, ProtectedLocalStorage localStorage, HttpClient httpClient, IAccountService accountService, IJwtService jwtService, AuthenticationStateProvider authStateProvider)
    {
        this.refreshTokenRepository = refreshTokenRepository;
        this.localStorage = localStorage;
        this.httpClient = httpClient;
        this.accountService = accountService;
        this.jwtService = jwtService;
        this.authStateProvider = authStateProvider;
    }

    public async Task<RefreshToken> GenerateRefreshToken(uint lHostId)
    {
        try
        {
            // Revoke any existing active refresh tokens for the host
            var existingTokens = await refreshTokenRepository.GetActiveRefreshTokensByHostIdAsync(lHostId);
            foreach (var token in existingTokens)
            {
                token.IsRevoked = true;
            }

            var newRefreshToken = new RefreshToken
            {
                Token = GenerateSecureToken(),
                // Set refresh token lifespan
                Expires = DateTime.UtcNow.AddDays(7),
                // Make the db do auto initialization
                Created = DateTime.UtcNow,
                IsRevoked = false,
                HostId = lHostId
            };

            // Save revoked tokens and the new token to the database
            await refreshTokenRepository.RevokeAndAddRefreshTokenAsync(existingTokens, newRefreshToken);


            return newRefreshToken;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while generating a new refresh token: {ex.Message}");
            throw;
        }
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

    // Implement a return type to give a meaningful feedback
    public async Task<TokenValidateResult> ValidateAndRefreshTokensAsync(string accessToken, string refreshToken)
    {
        try
        {

            if (string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(accessToken))
            {
                Console.WriteLine("No tokens found");
                return new TokenValidateResult
                {
                    IsSuccess = false,
                    Message = "No tokens found"
                };
            }

            // Check if the access token is valid
            if (!string.IsNullOrEmpty(accessToken) && !Extensions.IsTokenAlmostExpired(accessToken))
            {
                // Access token is valid, no need to refresh
                Constants.JWTToken = accessToken;
                return new TokenValidateResult
                {
                    IsSuccess = false,
                    Message = "The existing token is still Valid"
                };
            }

            // Validate the refresh token by retrieving it from the database
            var refreshTokenEntity = await accountService.GetRefreshTokenAsync(refreshToken);
            if (refreshTokenEntity == null)
            {
                Console.WriteLine("You're not authorized.");
                return new TokenValidateResult
                {
                    IsSuccess = false,
                    Message = "You're not authorized."
                };
            }

            var returnedToken = refreshTokenEntity.Data;

            if (returnedToken == null || !returnedToken.IsActive)
            {
                Console.WriteLine("Invalid or expired refresh token.");
                return new TokenValidateResult
                {
                    IsSuccess = false,
                    Message = "Invalid or expired refresh token."
                };
            }
            // AND We have an Active refreshToken in the 
            // Retrieve the host associated with this refresh token
            var response = await accountService.GetLHostByIdAsync(returnedToken.HostId);
            var lHost = response?.Data; 
            if (lHost == null)
            {
                Console.WriteLine("Associated host not found.");
                return new TokenValidateResult
                {
                    IsSuccess = false,
                    Message = "Associated host not found."
                };
            }
            // Generate new tokens
            var newAccessToken = jwtService.GenerateToken(lHost);
            var newRefreshToken = await GenerateRefreshToken(lHost.HostId);
            Constants.JWTToken = newAccessToken;
            await authStateProvider.GetAuthenticationStateAsync();
            Console.WriteLine("Tokens refreshed successfully.");
            return new TokenValidateResult
            {
                IsSuccess = true,
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Token
            };

        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred during token validation or refresh: {ex.Message}");
            return new TokenValidateResult
            {
                IsSuccess = false,
                Message = $"An error occurred during token validation or refresh: {ex.Message}"
            };
        }
    }
}


