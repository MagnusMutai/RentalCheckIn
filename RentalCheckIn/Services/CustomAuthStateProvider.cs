using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Text.Json;
namespace RentalCheckIn.Services;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService localStorage;
    private readonly HttpClient http;
    private bool isInitialized = false;
    private string token = string.Empty;

    public CustomAuthStateProvider(ILocalStorageService localStorage, HttpClient http)
    {
        this.localStorage = localStorage;
        this.http = http;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        //if (isInitialized)
        //{
        //}
        //token = await localStorage.GetItemAsStringAsync("token");
        string token = "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTUxMiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiVG9ueSBTdGFyayIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6Iklyb24gTWFuIiwiZXhwIjozMTY4NTQwMDAwfQ.IbVQa1lNYYOzwso69xYfsMOHnQfO3VLvVqV2SOXS7sTtyyZ8DEf5jmmwz2FGLJJvZnQKZuieHnmHkg7CGkDbvA";
        
        var identity = new ClaimsIdentity();
        http.DefaultRequestHeaders.Authorization = null;

        if (!string.IsNullOrEmpty(Constants.JWTToken))
        {
            identity = new ClaimsIdentity(ParseClaimsFromJwt(Constants.JWTToken), "jwt");
            http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", Constants.JWTToken.Replace("\"", ""));
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

    public async Task InitializeAsync()
    {
        isInitialized = true;
    }
}
