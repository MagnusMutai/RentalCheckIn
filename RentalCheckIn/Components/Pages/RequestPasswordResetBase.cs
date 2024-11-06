using Microsoft.AspNetCore.Components;
namespace RentalCheckIn.Components.Pages;

public class RequestPasswordResetBase : ComponentBase
{
    protected ResetRequestDto resetModel = new ResetRequestDto();
    protected bool isEmailSent = false;
    protected string ErrorMessage;

    [Inject]
    protected IAuthService AuthService { get; set; }

    protected async Task HandleValidSubmit()
    {
        var result = await AuthService.ForgotPasswordAsync(resetModel);
    }
}
