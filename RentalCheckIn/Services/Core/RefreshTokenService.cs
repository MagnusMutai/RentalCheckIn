namespace RentalCheckIn.Services.Core;

public class RefreshTokenService
{
    private readonly IRefreshTokenRepository refreshTokenRepository;
    private readonly ProtectedLocalStorage localStorage;
    private readonly HttpClient httpClient;
    private readonly IAccountService accountService;
    private readonly JwtService jwtService;
    private readonly AuthenticationStateProvider authStateProvider;

    public RefreshTokenService(IRefreshTokenRepository refreshTokenRepository, ProtectedLocalStorage localStorage, HttpClient httpClient, IAccountService accountService, JwtService jwtService, AuthenticationStateProvider authStateProvider)
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
    public async Task ValidateAndRefreshTokensAsync(string accessToken, string refreshToken)
    {
        try
        {

            // Check if the access token is valid
            if (!string.IsNullOrEmpty(accessToken) && !Extensions.IsTokenExpired(accessToken))
            {
                // Access token is valid, no need to refresh
                Constants.JWTToken = accessToken;
                return;
            }

            if (string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(accessToken))
            {
                Console.WriteLine("No tokens found, redirecting to login...");
                // Redirect to login 
                return;
            }


            // Validate the refresh token by retrieving it from the database
            var refreshTokenEntity = await accountService.GetRefreshTokenAsync(refreshToken);
            if (refreshTokenEntity == null)
            {
                Console.WriteLine("You're not authorized");
                return;
            }

            var returnedToken = refreshTokenEntity.RefreshToken;

            if (returnedToken == null || !returnedToken.IsActive)
            {
                Console.WriteLine("Invalid or expired refresh token.");
                return;
            }

            // Retrieve the host associated with this refresh token
            var host = await accountService.GetLHostByIdAsync(returnedToken.HostId);
            if (host == null)
            {
                Console.WriteLine("Associated host not found.");
                return;
            }

            // Generate new tokens
            var newAccessToken = jwtService.GenerateToken(host);
            var newRefreshToken = await GenerateRefreshToken(host.HostId);

            StoreToken("token", newAccessToken);
            StoreToken("refreshToken", newRefreshToken.Token);
            Console.WriteLine("Tokens refreshed successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred during token validation or refresh: {ex.Message}");
            return; // Indicate failure to the caller
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


