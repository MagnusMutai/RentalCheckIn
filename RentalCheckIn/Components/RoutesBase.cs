using Microsoft.AspNetCore.Components;
using System.Globalization;

namespace RentalCheckIn.Components;

public class RoutesBase : ComponentBase, IAsyncDisposable
{

    private bool isTokenRefreshed;
    private bool isDisposed;
    private bool isRefreshing = false;
    private PeriodicTimer? timer;
    private CancellationTokenSource? cts;
    [Inject]
    private AuthenticationStateProvider AuthStateProvider { get; set; }
    [Inject]
    private IAuthService AuthService { get; set; }
    [Inject]
    private ProtectedLocalStorage LocalStorage { get; set; }
    [Inject]
    private NavigationManager NavigationManager { get; set; }
    [Inject]
    private ILogger<RoutesBase> Logger { get; set; }
    protected override void OnInitialized()
    {
        // Subscribe to authentication state changes
        if (AuthStateProvider != null)
        {
            AuthStateProvider.AuthenticationStateChanged += async (task) =>
            {
                try
                {
                    await OnAuthenticationStateChanged(task);
                }
                catch (Exception ex) 
                {
                    Logger.LogError(ex, "An unexpected error has on OnAuthenticationStateChanged in Routes Component.");
                }
            };
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        try
        {
            if (!isDisposed)
            {

                if (firstRender && !isTokenRefreshed)
                {
                    isTokenRefreshed = true;
                    await StartRefreshTokenAsync();
                }
            }

            var response = await AuthService.RefreshTokenAsync();

            if (response.IsSuccess)
            {
                await LocalStorage.SetAsync("refreshToken", response.RefreshToken);
                await LocalStorage.SetAsync("token", response.AccessToken);
                Constants.JWTToken = response.AccessToken;

                // Notify the authentication state provider
                if (AuthStateProvider is CustomAuthStateProvider customAuthStateProvider)
                {
                    customAuthStateProvider.NotifyUserAuthentication(Constants.JWTToken);
                }
            }
            else
            {
                StopRefreshToken();
            }
        }
        catch(Exception ex)
        {
            Logger.LogError(ex, "An unexpected errror occurred while trying to refresh JWT tokens in Routes Component. FirstRender: {firstRender}, isTokenRefreshed: {isTokenRefreshed}", firstRender, isTokenRefreshed);
        }
    }

    private async Task OnAuthenticationStateChanged(Task<AuthenticationState> task)
    {
        try
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
        catch (Exception ex)
        {
            Logger.LogError(ex, "An unexpected error occurred while trying to set the timer state of Refreshing tokens in Routes Component.");
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
                        customAuthStateProvider.NotifyUserAuthentication(Constants.JWTToken);
                    }
                }
            }
        }
        catch (OperationCanceledException)
        {
            Logger.LogInformation("Token refresh loop was cancelled in Routes component.");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An unexpected error occurred while executing the token refresh timer loop.");
        }
        finally
        {
            isRefreshing = false;
        }
    }

    private void StopRefreshToken()
    {
        isDisposed = true;

        try
        {
            cts?.Cancel();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to cancel the CancellationTokenSource in StopRefreshToken in Routes component.");
        }

        try
        {
            timer?.Dispose();
        }
        catch (Exception ex) 
        {
            Logger.LogError(ex, "Failed to dispose the timer in StopRefreshToken.");
        }

        isRefreshing = false;
    }


    public async ValueTask DisposeAsync()
    {
        isDisposed = true;
        try
        {
            AuthStateProvider.AuthenticationStateChanged -= async (task) => await OnAuthenticationStateChanged(task);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to unsubscribe from AuthenticationStateChanged in DisposeAsync in Routes component.");
        }
        if (cts != null)
        {
            try
            {
                await cts.CancelAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to cancel the CancellationTokenSource in DisposeAsync in Routes component.");
            }
            try
            {
                cts.Dispose();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to dispose the CancellationTokenSource in DisposeAsync in Routes component.");
            }
            cts = null;
        }
        if (timer != null)
        {
            try
            {
                timer.Dispose();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to dispose the timer in DisposeAsync in Routes component.");
            }
            timer = null;
        }
    }
}
