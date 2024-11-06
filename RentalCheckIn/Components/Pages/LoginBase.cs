using Microsoft.AspNetCore.Components;
namespace RentalCheckIn.Components.Pages;

public class LoginBase : ComponentBase
{
    protected HostLoginDto loginModel = new();
    protected string ErrorMessage;
    [Inject]
    private ProtectedLocalStorage LocalStorage { get; set; }
    [Inject]
    private NavigationManager NavigationManager { get; set; }
    [Inject]
    private IAuthService AuthService { get; set; }

    protected async Task HandleLogin()
    {
        var result = await AuthService.LoginAsync(loginModel);
        if (result.Success)
        {
            var lHost = result.Host;
            // Store email for OTP verification
            await LocalStorage.SetAsync("emailForOtp", lHost.MailAddress);
            NavigationManager.NavigateTo("/verify-otp");
        }
        else
        {
            ErrorMessage = result.Message;
        }
    }
}
