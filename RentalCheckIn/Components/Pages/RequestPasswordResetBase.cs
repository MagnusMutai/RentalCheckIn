using Microsoft.AspNetCore.Components;
namespace RentalCheckIn.Components.Pages;

public class RequestPasswordResetBase : ComponentBase
{
    protected ResetRequestDto resetModel = new ResetRequestDto();
    protected bool isEmailSent = false;
    protected string Message;
    protected bool ShouldSpin;

    [Inject]
    protected IAuthService AuthService { get; set; }

    protected async Task HandleValidSubmit()
    {
        try
        {
            ShouldSpin = true;
            var result = await AuthService.ForgotPasswordAsync(resetModel);
            // In other similar pages remember to go back and check null first.
            if (result != null)
            {
                Message = result.Message;
            }
            ShouldSpin = false;
        }
        catch (Exception ex) 
        {
            Message = "An unexpected error has occurred. Please try again later";
        }
    }
}
