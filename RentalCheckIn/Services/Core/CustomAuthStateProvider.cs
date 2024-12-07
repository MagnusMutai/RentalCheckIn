using System.Net.Http.Headers;

namespace RentalCheckIn.Services.Core;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient httpClient;
    private readonly ILogger<CustomAuthStateProvider> logger;

    public CustomAuthStateProvider(HttpClient httpClient, ILogger<CustomAuthStateProvider> logger)
    {
        this.httpClient = httpClient;
        this.logger = logger;
    }

    // We have repetition here
    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var state = BuildAuthenticationState(Constants.JWTToken);
        NotifyAuthenticationStateChanged(Task.FromResult(state));
        return Task.FromResult(state);
    }

    public AuthenticationState NotifyUserAuthentication(string token)
    {
        var state = BuildAuthenticationState(token);
        NotifyAuthenticationStateChanged(Task.FromResult(state));
        return state;
    }

    public void NotifyUserLogout()
    {
        var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymousUser)));
    }

    private AuthenticationState BuildAuthenticationState(string token)
    {
        var identity = new ClaimsIdentity();
        httpClient.DefaultRequestHeaders.Authorization = null;

        try
        {
            if (!string.IsNullOrEmpty(token))
            {
                var tokenExpired = Extensions.IsTokenExpired(token);
                if (!tokenExpired)
                {
                    identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
                    httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token.Replace("\"", ""));
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred in CustomAuthStateProvider while trying to process the authentication state.");
        }

        var user = new ClaimsPrincipal(identity);
        return new AuthenticationState(user);
    }

    public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
        return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
    }

    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }

        return Convert.FromBase64String(base64);
    }
}
