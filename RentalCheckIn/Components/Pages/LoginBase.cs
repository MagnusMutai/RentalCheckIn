using Microsoft.AspNetCore.Components;
namespace RentalCheckIn.Components.Pages;

public class LoginBase : ComponentBase
{
    protected HostLoginDTO loginModel = new();
    protected string ErrorMessage;
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

    protected override async Task OnInitializedAsync()
    {
        // Get the current authentication state
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();

        // Check if the user is authenticated
        if (authState.User.Identity is { IsAuthenticated: true } )
        {
            // Redirect to the home page if the user is already logged in
            NavigationManager.NavigateTo("/", forceLoad:true);
        }
    }

    protected async Task HandleLogin()
    {
        IsRegistering = true;
        try
        {
            var result = await AuthService.LoginAsync(loginModel);
            if (result.IsSuccess)
            {
                var lHost = result.Data;


                // Redirect based on the selected 2FA method
                if (lHost.Selected2FA.Equals("TOTP", StringComparison.OrdinalIgnoreCase))
                {
                    // Store email for OTP verification
                    await LocalStorage.SetAsync("emailForOtp", lHost.MailAddress);
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
                    ErrorMessage = "Unsupported 2FA method selected.";
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
            ErrorMessage = "An unexpected error occurred";
        }
        IsRegistering = false;
    }

    protected void HandleCloseToast()
    {
        DisplayToast = null;
    }

}
