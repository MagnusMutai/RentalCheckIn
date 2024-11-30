using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using RentalCheckIn.Locales;
namespace RentalCheckIn.Components.Pages;

public class RequestPasswordResetBase : ComponentBase
{
    protected ResetRequestDTO resetModel = new ResetRequestDTO();
    protected bool isEmailSent = false;
    protected string? Message;
    protected bool IsRegistering;

    [Inject]
    protected IAuthService AuthService { get; set; }
    [Inject]
    private ILogger<RequestPasswordResetBase> Logger { get; set; }

    [Inject]
    protected IStringLocalizer<Resource> Localizer { get; set; }

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
            Message = Localizer["UnexpectedErrorOccurred"];
            Logger.LogError(ex, "An unexpected error occurred while requeting password reset in RequestPasswordReset component");
        }
    }
}
