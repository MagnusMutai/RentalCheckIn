using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using RentalCheckIn.Locales;
namespace RentalCheckIn.Components.Pages;

public class LoginBase : ComponentBase
{
    protected HostLoginDTO loginModel = new();
    protected string? ErrorMessage;
    protected bool IsRegistering;
    protected string? DisplayToast { get; set; } = "d-block";
    [Inject]
    private ProtectedLocalStorage LocalStorage { get; set; }
    [Inject]
    private NavigationManager NavigationManager { get; set; }
    [Inject]
    private IAuthService AuthService { get; set; }
    [Inject]
    private AuthenticationStateProvider AuthStateProvider { get; set; }
    [Inject]
    private ILogger<LoginBase> Logger { get; set; }
    [Inject]
    protected IStringLocalizer<Resource> Localizer { get; set; }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            // Get the current authentication state
            var authState = await AuthStateProvider.GetAuthenticationStateAsync();

            // Check if the user is authenticated
            if (authState.User.Identity is { IsAuthenticated: true })
            {
                // Redirect to the home page if the user is already logged in
                NavigationManager.NavigateTo("/", forceLoad: true);
            }
        }
        catch (Exception ex) 
        {
            ErrorMessage = Localizer["UnexpectedErrorOccurred"];
            Logger.LogError(ex, "An unexpected error occurred while getting the state of authentication of the user in Login page.");
        }
    }

    protected async Task HandleLogin()
    {
        try
        {
            IsRegistering = true;
    
            var result = await AuthService.LoginAsync(loginModel);
            
            if (result.IsSuccess)
            {
                var lHost = result.Data;

                // Redirect based on the selected 2FA method
                if (lHost.Selected2FA.Equals("TOTP", StringComparison.OrdinalIgnoreCase))
                {
                    // Store email for OTP verification
                    await LocalStorage.SetAsync("emailForTOTP", lHost.MailAddress);
                    NavigationManager.NavigateTo("/verify-otp");
                }
                else if (lHost.Selected2FA.Equals("FaceID", StringComparison.OrdinalIgnoreCase))
                {
                    // Store necessary user ID temporarily for 2FA
                    await LocalStorage.SetAsync("UserIdFor2FA", lHost.HostId);
                    NavigationManager.NavigateTo("/verify-faceid");
                }
                else
                {
                    // Handle cases where no 2FA is selected or unsupported method
                    ErrorMessage = Localizer["Unsupported2FAMethod"];
                    DisplayToast = "d-block";
                }
            }
            else
            {
                ErrorMessage = result.Message;
                DisplayToast = DisplayToast ?? "d-block";
            }
        }
        catch (Exception ex) 
        {
            ErrorMessage = Localizer["UnexpectedErrorOccurred"];
            Logger.LogError(ex, "An unexpected error occurred in the login page while trying to login a user.");
        }
        finally
        {
            IsRegistering = false;
        }
    }

    protected void HandleCloseToast()
    {
        DisplayToast = null;
    }

}
