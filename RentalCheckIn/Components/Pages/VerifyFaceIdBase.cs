using Microsoft.AspNetCore.Components;

namespace RentalCheckIn.Components.Pages;

public class VerifyFaceIdBase : ComponentBase
{
    protected string? ErrorMessage;
    protected bool IsAuthenticating;
    protected bool shouldDisplayAuthButton;

    protected string? DisplayToast { get; set; } = "d-block";
    [Inject]
    private IAuthService AuthService { get; set; }
    [Inject]
    private ProtectedLocalStorage LocalStorage { get; set; }
    [Inject]
    private ILogger<VerifyFaceIdBase> Logger { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await Authenticate();
    }

    protected async Task Authenticate()
    {

        try
        {
            IsAuthenticating = true;
            ErrorMessage = string.Empty;
            var result = await AuthService.AuthenticateFaceIdAsync();

            if (result.IsSuccess)
            {
                shouldDisplayAuthButton = false;
                await LocalStorage.DeleteAsync("UserIdFor2FA");
            }
            else 
            {
                shouldDisplayAuthButton = true;
                ErrorMessage = result.Message;
            }
        }
        catch(Exception ex) 
        {
            ErrorMessage = $"An unexpected error has occurred, please try again later.";
            Logger.LogError(ex, "An unexpected error occurred while trying to verify face Id in VerifyFaceIdComponent.");
        }
        finally
        {
            IsAuthenticating = false;
        }
    }

    protected void HandleCloseToast()
    {
        DisplayToast = null;
    }
}
