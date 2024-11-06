using Microsoft.AspNetCore.Components;

namespace RentalCheckIn.Components.Pages;
public class RegisterBase : ComponentBase
{
    protected HostSignUpDto registerModel = new();
    protected string ErrorMessage;
    protected string SuccessMessage;
    protected string TotpSecret;
    [Inject]
    protected IAuthService AuthService { get; set; }

    protected async Task HandleRegister()
    {
        var result = await AuthService.RegisterAsync(registerModel);

        if (result.Success)
        {
            TotpSecret = result.Host.TotpSecret;
            SuccessMessage = "You are successfully registered, check your email to confirm your account";
        }

        ErrorMessage = result.Message;
    }
}
