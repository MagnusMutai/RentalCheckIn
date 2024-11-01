using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using OtpNet;
using System.Net.Http.Headers;
using System.Text.Json;
using static System.Net.WebRequestMethods;
namespace RentalCheckIn.Services;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService localStorage;
    private readonly LoginService loginService;
    private readonly HttpClient http;

    public CustomAuthStateProvider(ILocalStorageService localStorage, LoginService loginService, HttpClient http) 
    {
        this.localStorage = localStorage;
        this.loginService = loginService;
        this.http = http;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var savedToken = loginService.GetToken();

        //var savedToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiIzIiwiZW1haWwiOiJNYWdudXNNbkBnbWFpbC5jb20iLCJuYmYiOjE3MzA0ODczMTYsImV4cCI6MTczMTA5MjExNiwiaWF0IjoxNzMwNDg3MzE2fQ.QJL_y9IX1iW2ZjJSyYuBUnG1xwFB5N2xOzLEAia3i2k";

        var identity = new ClaimsIdentity();
        http.DefaultRequestHeaders.Authorization = null;

        if (!string.IsNullOrEmpty(savedToken))
        {
            identity = new ClaimsIdentity(ParseClaimsFromJwt(savedToken), "jwt");
            http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", savedToken.Replace("\"", ""));
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

    public void NotifyUserAuthentication(ClaimsPrincipal user)
    {
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    public async Task NotifyUserLogout()
    {
        await localStorage.RemoveItemAsync("authToken");
        var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymousUser)));
    }
}
