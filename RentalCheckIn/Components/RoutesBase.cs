using Microsoft.AspNetCore.Components;

namespace RentalCheckIn.Components;

public class RoutesBase : ComponentBase, IAsyncDisposable
{

    private bool isTokenRefreshed;
    private bool isFirstTokenRefreshed;
    private bool isDisposed;
    private Timer tokenRefreshTimer;
    private bool isRefreshing = false;
    private PeriodicTimer timer;
    private CancellationTokenSource cts;
    [Inject]
    private AuthenticationStateProvider AuthStateProvider { get; set; }
    [Inject]
    private IAuthService AuthService { get; set; }
    [Inject]
    private ProtectedLocalStorage LocalStorage { get; set; }
    [Inject]
    private NavigationManager NavigationManager { get; set; }

    protected override void OnInitialized()
    {
        // Subscribe to authentication state changes
        AuthStateProvider.AuthenticationStateChanged += async (task) => await OnAuthenticationStateChanged(task);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!isDisposed)
        {

            if (firstRender && !isTokenRefreshed)
            {
                isTokenRefreshed = true;
                await StartRefreshTokenAsync();
            }

            if (!isFirstTokenRefreshed)
            {
                isFirstTokenRefreshed = true;

                var response = await AuthService.RefreshTokenAsync();

                if (response.IsSuccess)
                {
                    await LocalStorage.SetAsync("refreshToken", response.RefreshToken);
                    await LocalStorage.SetAsync("token", response.AccessToken);
                    Constants.JWTToken = response.AccessToken;

                    // Notify the authentication state provider
                    if (AuthStateProvider is CustomAuthStateProvider customAuthStateProvider)
                    {
                        await customAuthStateProvider.NotifyUserAuthentication(Constants.JWTToken);
                    }
                }
                else
                {
                    Console.WriteLine("Not authorized");
                    StopRefreshToken();
                }
            }
        }
    }

    private async Task OnAuthenticationStateChanged(Task<AuthenticationState> task)
    {
        var authState = await task;
        var isAuthenticated = authState.User.Identity?.IsAuthenticated == true;

        if (isAuthenticated)
        {
            // User has logged in, start the token refresh loop
            await StartRefreshTokenAsync();
        }
        else
        {
            // User has logged out, stop the token refresh loop
            StopRefreshToken();
        }
    }

    private async Task StartRefreshTokenAsync()
    {
        if (isRefreshing)
            return;

        isRefreshing = true;
        try
        {

            cts = new CancellationTokenSource();
            timer = new PeriodicTimer(TimeSpan.FromMinutes(15));

            while (await timer.WaitForNextTickAsync(cts.Token))
            {

                var response = await AuthService.RefreshTokenAsync();
                if (response.IsSuccess)
                {
                    await LocalStorage.SetAsync("refreshToken", response.RefreshToken);
                    await LocalStorage.SetAsync("token", response.AccessToken);
                    Constants.JWTToken = response.AccessToken;

                    // Notify the authentication state provider
                    if (AuthStateProvider is CustomAuthStateProvider customAuthStateProvider)
                    {
                        await customAuthStateProvider.NotifyUserAuthentication(Constants.JWTToken);
                    }
                }
                else
                {
                    Console.WriteLine($"Token refresh failed {response.Message}.");
                }
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Timer was canceled");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error in token refresh loop: {ex.Message}");
        }
        finally
        {
            isRefreshing = false;
        }
    }

    private void StopRefreshToken()
    {
        isDisposed = true;
        cts?.Cancel();
        timer?.Dispose();
        isRefreshing = false;
    }


    public async ValueTask DisposeAsync()
    {
        isDisposed = true;
        AuthStateProvider.AuthenticationStateChanged -= async (task) => await OnAuthenticationStateChanged(task);

        if (cts != null)
        {
         await cts.CancelAsync();
            cts.Dispose();
            cts = null;
        }
        if (timer != null)
        {
            timer.Dispose();
            timer = null;
        }
    }
}
