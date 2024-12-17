using Microsoft.AspNetCore.Components;
using System.Globalization;
namespace RentalCheckIn.Components;

public class RoutesBase : ComponentBase, IAsyncDisposable
{
    private bool IsTokenRefreshed { get; set; }
    private bool IsDisposed { get; set; }
    private bool IsRefreshing { get; set; }
    private PeriodicTimer? Timer { get; set; }
    private CancellationTokenSource? Cts { get; set; }
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
        // Set the culture for use in the service classes
        CultureUtils.CurCulture = CultureInfo.CurrentCulture;

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
            if (firstRender)
            {
                if (!IsDisposed && !IsTokenRefreshed)
                {
                    IsTokenRefreshed = true;
                    await StartRefreshTokenAsync();
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
                        await customAuthStateProvider.GetAuthenticationStateAsync();

                        if (Constants.JWTToken != null)
                        {
                            NavigationManager.NavigateTo("/");
                        }
                    }
                }
                else
                {
                    StopRefreshToken();
                }
            }
        }
        catch(Exception ex)
        {
            Logger.LogError(ex, "An unexpected errror occurred while trying to refresh JWT tokens in Routes Component. FirstRender: {firstRender}, IsTokenRefreshed: {IsTokenRefreshed}", firstRender, IsTokenRefreshed);
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
        if (IsRefreshing)
            return;

        IsRefreshing = true;

        try
        {
            // Monitor this check for backward compatibility.
            if (AuthService == null || LocalStorage == null || AuthStateProvider == null)
            {
                return;
            }

            Cts = new CancellationTokenSource();
            Timer = new PeriodicTimer(TimeSpan.FromMinutes(15));

            while (await Timer.WaitForNextTickAsync(Cts.Token))
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
                        await customAuthStateProvider.GetAuthenticationStateAsync();
                    }
                }
            }
        }
        catch (OperationCanceledException ex)
        {
            Logger.LogDebug(ex, "Token refresh loop was cancelled in Routes component.");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An unexpected error occurred while executing the token refresh timer loop.");
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    private void StopRefreshToken()
    {
        IsDisposed = true;

        try
        {
            Cts?.Cancel();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to cancel the CancellationTokenSource in StopRefreshToken in Routes component.");
        }

        try
        {
            Timer?.Dispose();
        }
        catch (Exception ex) 
        {
            Logger.LogError(ex, "Failed to dispose the timer in StopRefreshToken.");
        }

        IsRefreshing = false;
    }


    public async ValueTask DisposeAsync()
    {
        IsDisposed = true;
        try
        {
            AuthStateProvider.AuthenticationStateChanged -= async (task) => await OnAuthenticationStateChanged(task);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to unsubscribe from AuthenticationStateChanged in DisposeAsync in Routes component.");
        }
        if (Cts != null)
        {
            try
            {
                await Cts.CancelAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to cancel the CancellationTokenSource in DisposeAsync in Routes component.");
            }
            try
            {
                Cts.Dispose();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to dispose the CancellationTokenSource in DisposeAsync in Routes component.");
            }
            Cts = null;
        }
        if (Timer != null)
        {
            try
            {
                Timer.Dispose();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to dispose the timer in DisposeAsync in Routes component.");
            }
            Timer = null;
        }
    }
}
