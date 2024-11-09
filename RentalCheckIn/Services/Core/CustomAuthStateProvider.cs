using System.Net.Http.Headers;

namespace RentalCheckIn.Services.Core;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient http;

    public CustomAuthStateProvider(HttpClient http)
    {
        this.http = http;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {

        var identity = new ClaimsIdentity();
        http.DefaultRequestHeaders.Authorization = null;
        
        if (!string.IsNullOrEmpty(Constants.JWTToken))
        { 
            var tokenExpired = Extensions.IsTokenExpired(Constants.JWTToken);
            if (!tokenExpired) 
            {
                identity = new ClaimsIdentity(ParseClaimsFromJwt(Constants.JWTToken), "jwt");
                http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", Constants.JWTToken.Replace("\"", ""));
            }
        }

        var user = new ClaimsPrincipal(identity);
        var state = new AuthenticationState(user);

        NotifyAuthenticationStateChanged(Task.FromResult(state));

        return state;
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
