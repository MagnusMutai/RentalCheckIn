using Microsoft.AspNetCore.Components;

namespace RentalCheckIn.Components.Pages;

public class VerifyFaceIdBase : ComponentBase
{
    protected string ErrorMessage;
    protected bool IsAuthenticating;
    protected bool shouldDisplayAuthButton;

    protected string? DisplayToast { get; set; } = "d-block";
    [Inject]
    private IAuthService AuthService { get; set; }


    protected override async Task OnInitializedAsync()
    {
        await Authenticate();
    }

    protected async Task Authenticate()
    {
        IsAuthenticating = true;
        ErrorMessage = string.Empty;

        try
        {
            var result = await AuthService.AuthenticateFaceIdAsync();

            if (!result.IsSuccess)
            {
                shouldDisplayAuthButton = true;
                ErrorMessage = result.Message;
            }
            else 
            {
                shouldDisplayAuthButton = false;
            }
        }
        catch
        {
            ErrorMessage = $"An unexpected error has occurred, please try again later.";
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
