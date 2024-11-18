using Microsoft.AspNetCore.Components;
namespace RentalCheckIn.Components.Pages;

public class RequestPasswordResetBase : ComponentBase
{
    protected ResetRequestDTO resetModel = new ResetRequestDTO();
    protected bool isEmailSent = false;
    protected string Message;
    protected bool IsRegistering;

    [Inject]
    protected IAuthService AuthService { get; set; }

    protected async Task HandleValidSubmit()
    {
        try
        {
            IsRegistering = true;
            var result = await AuthService.ForgotPasswordAsync(resetModel);
            // In other similar pages remember to go back and check null first.
            if (result != null)
            {
                Message = result.Message;
            }
            IsRegistering = false;
        }
        catch (Exception ex) 
        {
            Message = "An unexpected error has occurred. Please try again later";
        }
    }
}
